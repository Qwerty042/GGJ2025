using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject tileSelectBoarderPrefab;
    public Vector3[] tileSpots;// = new Vector3[7];
    // public Sprite sp;
    public AudioSource sfx;

    private enum GameState
    {
        gsPRE_START,
        gsLOAD_LEVEL,
        gsSELECTING,
        gsSWAPPING,
        gsCHECK_ORDER
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

    private int nextLevel = 0;
    private List<OrderPermutation> orderPermutations = new List<OrderPermutation>();
    private List<LevelMetadata> levelMetadatas = new List<LevelMetadata>();
    private GameObject[] tiles = new GameObject[7];
    private GameObject[] tileSelectBoarders = new GameObject[2] {null, null};
    private GameObject[] swapTiles = new GameObject[2] {null, null};
    private Vector3[] tileDestinations = new Vector3[2];
    private bool doSwap = false;
    private const float SWAP_MOVE_TIME = 0.25f;
    private const float SWAP_AMPLITUDE = 2f;
    private float swapTimer = 0f;

    void Start()
    {
        for (int i = 0; i < tileSpots.Length; i++)
        {
            tiles[i] = Instantiate(tileSelectBoarderPrefab, tileSpots[i], Quaternion.identity);
        }

        LoadPermutations();
        LoadLevelMetadata();

        // Sprite sp0 = Resources.Load<Sprite>("Levels/0-0");
        // Sprite sp1 = Resources.Load<Sprite>("Levels/0-1");
        // Sprite sp2 = Resources.Load<Sprite>("Levels/0-2");

        // Debug.Log("Hello World!");
        // tiles[0].GetComponent<SpriteRenderer>().sprite = sp0;
        // tiles[0].GetComponent<TileProperties>().correctIndex = 6;
        // tiles[1].GetComponent<SpriteRenderer>().sprite = sp1;
        // tiles[2].GetComponent<SpriteRenderer>().sprite = sp2;
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.gsPRE_START:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    gameState = GameState.gsLOAD_LEVEL;
                }
                break;
            case GameState.gsLOAD_LEVEL:
                LoadLevel(nextLevel);
                nextLevel++;
                break;
            case GameState.gsSELECTING:
                CheckSwapping();
                if(doSwap)
                {
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
                    swapTiles[0] = hit.transform.gameObject;
                    tileSelectBoarders[0] = Instantiate(tileSelectBoarderPrefab, swapTiles[0].transform.position, Quaternion.identity);
                }
                else if (swapTiles[0].GetInstanceID() == hit.transform.gameObject.GetInstanceID()) // Tile is already selected, deselect:
                {
                    ClearTileSelection();
                }
                else
                {
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
}
