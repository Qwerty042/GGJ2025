using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject tileSelectBoarderPrefab;
    public GameObject tileValidityBoarderPrefab;
    public Vector3[] tileSpots;// = new Vector3[7];
    // public Sprite sp;
    public AudioSource sfxSelect;
    public AudioSource sfxSwap;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI swapCountText;
    public TextMeshProUGUI scoreText;

    private enum GameState
    {
        gsPRE_START,
        gsLOAD_LEVEL,
        gsSELECTING,
        gsSWAPPING,
        gsCHECK_ORDER,
        gsCLEAN_UP
    }

    private struct OrderPermutation
    {
        public int[] order;
        public int minSwaps;
        public int bubbleSwaps;
    }

    private struct LevelMetadata
    {
        public int level;
        public string main_title;
        public string left_title;
        public string right_title;
    }

    private GameState gameState = GameState.gsPRE_START;
    public const int TOTAL_LEVELS = 47;
    private int nextLevel;
    private int levelBubbleSwaps = 99;
    private int levelMinSwaps = 99;
    private List<OrderPermutation> orderPermutations = new List<OrderPermutation>();
    private List<LevelMetadata> levelMetadatas = new List<LevelMetadata>();
    private GameObject[] tiles = new GameObject[7];
    private GameObject[] tileSelectBoarders = new GameObject[2] {null, null};
    private GameObject[] tileValidityBoarders = new GameObject[7];
    private GameObject[] swapTiles = new GameObject[2] {null, null};
    private Vector3[] tileDestinations = new Vector3[2];
    private bool doSwap = false;
    private const float SWAP_MOVE_TIME = 0.25f;
    private const float SWAP_AMPLITUDE = 2f;
    private float swapTimer = 0f;
    private float successTimer = 0f;
    private float successPauseTime = 2f;
    static System.Random rnd = new System.Random();
    private List<int> doneLevelList = new List<int>();
    private OrderPermutation currentPermutation;
    private int swapCount;
    private int score = 0;

    void Start()
    {
        for (int i = 0; i < tileSpots.Length; i++)
        {
            tiles[i] = Instantiate(tileSelectBoarderPrefab, tileSpots[i], Quaternion.identity);
        }

        LoadPermutations();
        LoadLevelMetadata();
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.gsPRE_START:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    for (int i = 0; i < tileSpots.Length; i++)
                    {
                        Destroy(tiles[i]);
                    }
                    gameState = GameState.gsLOAD_LEVEL;
                }
                break;
            case GameState.gsLOAD_LEVEL:
                nextLevel = rnd.Next(TOTAL_LEVELS);
                while (doneLevelList.Contains(nextLevel))
                {
                    nextLevel = rnd.Next(TOTAL_LEVELS);
                }
                Debug.Log($"Loading level {nextLevel}...");
                LoadLevel(nextLevel);
                doneLevelList.Add(nextLevel);

                nextLevel++;
                gameState = GameState.gsSELECTING;
                break;
            case GameState.gsSELECTING:
                CheckSwapping();
                if(doSwap)
                {
                    int firstIndex = swapTiles[0].GetComponent<TileProperties>().index;
                    swapTiles[0].GetComponent<TileProperties>().index = swapTiles[1].GetComponent<TileProperties>().index;
                    swapTiles[1].GetComponent<TileProperties>().index = firstIndex;
                    tileDestinations[0] = swapTiles[1].transform.position;
                    tileDestinations[1] = swapTiles[0].transform.position;
                    gameState = GameState.gsSWAPPING;
                }
                break;
            case GameState.gsSWAPPING:
                Swap();
                if (!doSwap)
                {
                    ClearTileSelection();
                    gameState = GameState.gsCHECK_ORDER;
                }
                break;
            case GameState.gsCHECK_ORDER:
                if (ValidateOrder())
                {
                    if (swapCount == currentPermutation.minSwaps)
                    {
                        // PERFECT!
                        score += 1000;
                    }
                    else if (swapCount > currentPermutation.bubbleSwaps)
                    {
                        // FAILURE!
                    }
                    else
                    {
                        score += 1000 - (1000 * (swapCount - currentPermutation.minSwaps) / (currentPermutation.bubbleSwaps + 1 - currentPermutation.minSwaps));
                    }
                    scoreText.text = $"Score: {score}";
                    for(int i = 0; i < tileSpots.Length; i++)
                    {
                        tileValidityBoarders[i] = Instantiate(tileValidityBoarderPrefab, tileSpots[i], Quaternion.identity);
                    }
                    gameState = GameState.gsCLEAN_UP;
                }
                else
                {
                    gameState = GameState.gsSELECTING;
                }
                break;
            case GameState.gsCLEAN_UP:
                successTimer += Time.deltaTime;
                if (successTimer >= successPauseTime) // Wait for a moment before cleaning up and moving to next level
                {
                    DestroyLevelCreatedInstances();
                    successTimer = 0f;
                    gameState = GameState.gsLOAD_LEVEL;
                }
                break;
        }
    }

    void LoadPermutations()
    {
        StreamReader fp = new StreamReader("Assets/Resources/Levels/all_permutations.csv");
        fp.ReadLine(); // Ignore first line which has the column titles
        string line = fp.ReadLine();
        while (line != null)
        {
            OrderPermutation permutation = new OrderPermutation();
            string[] cols = line.Split(';');
            permutation.order = Array.ConvertAll(cols[0].Split(','), int.Parse);
            permutation.minSwaps = int.Parse(cols[1]);
            permutation.bubbleSwaps = int.Parse(cols[2]);
            orderPermutations.Add(permutation);
            line = fp.ReadLine();
        }
        // OrderPermutation permutation1 = orderPermutations[3000];
        // Debug.Log($"{permutation1.order[0]},{permutation1.order[1]},{permutation1.order[2]},{permutation1.order[3]},{permutation1.order[4]},{permutation1.order[5]},{permutation1.order[6]} - {permutation1.minSwaps} - {permutation1.bubbleSwaps}");
    }

    void LoadLevelMetadata()
    {
        StreamReader fp = new StreamReader("Assets/Resources/Levels/levels.csv");
        string line = fp.ReadLine();
        while (line != null)
        {
            LevelMetadata levelMetadata = new LevelMetadata();
            string[] cols = line.Split(',');
            levelMetadata.level = int.Parse(cols[0]);
            levelMetadata.main_title = cols[1];
            levelMetadata.left_title = cols[2];
            levelMetadata.right_title = cols[3];
            levelMetadatas.Add(levelMetadata);
            line = fp.ReadLine();
            Debug.Log($"{levelMetadata.level} - {levelMetadata.main_title} - {levelMetadata.left_title} - {levelMetadata.right_title}");            
        }
    }

    void LoadLevel(int level)
    {
        currentPermutation = orderPermutations[rnd.Next(orderPermutations.Count)];
        Debug.Log($"level permutation picked: {currentPermutation.order[0]},{currentPermutation.order[1]},{currentPermutation.order[2]},{currentPermutation.order[3]},{currentPermutation.order[4]},{currentPermutation.order[5]},{currentPermutation.order[6]} - {currentPermutation.minSwaps} - {currentPermutation.bubbleSwaps}");
        levelMinSwaps = currentPermutation.minSwaps;
        levelBubbleSwaps = currentPermutation.bubbleSwaps;

        for (int i = 0; i < tileSpots.Length; i++)
        {
            tiles[i] = Instantiate(tilePrefab, tileSpots[i], Quaternion.identity);
            Sprite sp = Resources.Load<Sprite>($"Levels/{level}-{currentPermutation.order[i]}");
            tiles[i].GetComponent<SpriteRenderer>().sprite = sp;
            TileProperties tileProps = tiles[i].GetComponent<TileProperties>();
            tileProps.index = i;
            tileProps.correctIndex = currentPermutation.order[i];
        }

        titleText.text = levelMetadatas[nextLevel].main_title;
        leftText.text = levelMetadatas[nextLevel].left_title;
        rightText.text = levelMetadatas[nextLevel].right_title;
        swapCount = 0;
        swapCountText.text = $"You: 0\nBubble Sort: {currentPermutation.bubbleSwaps}\nOptimum: {currentPermutation.minSwaps}";


    }

    void CheckSwapping()
    {
        if(Input.GetMouseButtonDown(1)) // Right click anywhere will clear selection
        {
            ClearTileSelection();
        }
        if(Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if(hit.transform == null) // Didn't click anything
            {
                // ClearTileSelection(); Decide if to keep this or not based on play testing
            }
            else if(hit.transform.name.StartsWith(tilePrefab.name))
            {
                if(swapTiles[0] == null) // No other tile selected
                {
                    sfxSelect.Play();
                    swapTiles[0] = hit.transform.gameObject;
                    tileSelectBoarders[0] = Instantiate(tileSelectBoarderPrefab, swapTiles[0].transform.position, Quaternion.identity);
                }
                else if (swapTiles[0].GetInstanceID() == hit.transform.gameObject.GetInstanceID()) // Tile is already selected, deselect:
                {
                    ClearTileSelection();
                }
                else
                {
                    sfxSwap.Play();
                    swapCount++;
                    swapCountText.text = $"You: {swapCount}\nBubble Sort: {currentPermutation.bubbleSwaps}\nOptimum: {currentPermutation.minSwaps}";
                    swapTiles[1] = hit.transform.gameObject;
                    tileSelectBoarders[1] = Instantiate(tileSelectBoarderPrefab, swapTiles[1].transform.position, Quaternion.identity);
                    doSwap = true;
                }
            }
        }
    }

    void ClearTileSelection()
    {
        swapTiles = new GameObject[2] {null, null};
        Destroy(tileSelectBoarders[0]);
        Destroy(tileSelectBoarders[1]);
    }

    void Swap()
    {
        swapTimer += Time.deltaTime;

        float animationProgress = swapTimer/SWAP_MOVE_TIME;

        SwapMoveStepSine(animationProgress, 0);
        SwapMoveStepSine(animationProgress, 1);

        if(animationProgress > 1f)
        {
            swapTimer = 0;
            doSwap = false;
        }
    }


    //TODO: make the tiles spin as they fly
    void SwapMoveStepSine(float progressFraction, int tileIndex)
    {
        Vector3 nextPos = swapTiles[tileIndex].transform.position;
        float startX = tileSelectBoarders[tileIndex].transform.position.x;
        float xDist = startX - tileDestinations[tileIndex].x;
        float direction = tileIndex == 0 ? (1f) : (-1f);

        nextPos.x = Mathf.Lerp(startX, tileDestinations[tileIndex].x, progressFraction);
        nextPos.y = direction * SWAP_AMPLITUDE * Mathf.Sin(((startX * Mathf.PI)/(xDist))-((nextPos.x * Mathf.PI)/(xDist)));

        swapTiles[tileIndex].transform.position = nextPos;
    }


    bool ValidateOrder()
    {
        for(int i = 0; i < tiles.Length - 1; i++) // Don't need to check the last one because if all the others are correct, the last one has to be correct
        {
            TileProperties tileProps = tiles[i].GetComponent<TileProperties>();
            if(tileProps.index != tileProps.correctIndex)
            {
                return false;
            }
        }
        return true;
    }


    void DestroyLevelCreatedInstances()
    {
        for(int i = 0; i < tiles.Length; i++)
            if(tiles[i] != null)
                Destroy(tiles[i]);

        for(int i = 0; i < tileSpots.Length; i++)
            if(tileValidityBoarders[i] != null)
                Destroy(tileValidityBoarders[i]);
    }
}
