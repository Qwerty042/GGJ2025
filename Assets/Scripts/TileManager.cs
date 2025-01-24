using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject tileSelectBoarderPrefab;
    public Vector3[] tileSpots;// = new Vector3[7];

    private enum GameState
    {
        gsSELECTING,
        gsSWAPPING
    }

    // private struct Tile
    // {
    //     GameObject tileObject;
    //     int goalIndex;
    //     int currentIndex;
    // }

    private GameState gameState = GameState.gsSELECTING;

    private GameObject[] tiles;
    private GameObject[] tileSelectBoarders = new GameObject[2] {null, null};
    private GameObject[] swapTiles = new GameObject[2] {null, null};
    private Vector3[] tileDestinations = new Vector3[2];
    private bool doSwap = false;
    private int NUM_TILES;
    private const float SWAP_MOVE_TIME = 0.25f;
    private const float SWAP_AMPLITUDE = 2f;
    private float swapTimer = 0f;
    // private const int NUM_PERMUTATIONS = 5018;

    void Awake()
    {
        NUM_TILES = tileSpots.Length;

        tiles = new GameObject[NUM_TILES];
        for (int i = 0; i < NUM_TILES; i++)
        {
            tiles[i] = Instantiate(tilePrefab, tileSpots[i], Quaternion.identity);
        }
    }

    void Start()
    {
        Debug.Log("Hello World!");
    }

    void Update()
    {
        switch (gameState)
        {
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
                    gameState = GameState.gsSELECTING;
                }
                break;
        }
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
                    Debug.LogFormat("Selected Tile! Name:{0} ID:{1}", swapTiles[0].name, swapTiles[0].GetInstanceID().ToString());
                }
                else if (swapTiles[0].GetInstanceID() == hit.transform.gameObject.GetInstanceID()) // Tile is already selected, deselect:
                {
                    ClearTileSelection();
                }
                else
                {
                    swapTiles[1] = hit.transform.gameObject;
                    tileSelectBoarders[1] = Instantiate(tileSelectBoarderPrefab, swapTiles[1].transform.position, Quaternion.identity);
                    Debug.LogFormat("Swap Name_0:{0},ID_0:{1} with Name_1:{2},ID_1:{3}", swapTiles[0].name, 
                                                                                         swapTiles[0].GetInstanceID().ToString(),
                                                                                         swapTiles[1].name,
                                                                                         swapTiles[1].GetInstanceID().ToString());
                    doSwap = true;
                }
            }
        }
    }

    void ClearTileSelection()
    {
        Debug.Log("Clear selected");
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
            Debug.Log("Done swap animation");
            swapTimer = 0;
            doSwap = false;
        }
    }

    void SwapMoveStepSine(float progressFraction, int tileIndex)
    {
        Vector3 nextPos = swapTiles[tileIndex].transform.position;
        float startX = tileSelectBoarders[tileIndex].transform.position.x;
        float xDist = startX - tileDestinations[tileIndex].x;
        float direction = tileIndex == 0 ? (1f) : (-1f);

        nextPos.x = Mathf.Lerp(startX, tileDestinations[tileIndex].x, progressFraction);
        nextPos.y = direction * SWAP_AMPLITUDE * Mathf.Sin(((startX * Mathf.PI)/(xDist))-((nextPos.x * Mathf.PI)/(xDist)));
        if(tileIndex == 0)
        {
            Debug.LogFormat("X:{0},Y:{1}", nextPos.x, nextPos.y);
        }
        swapTiles[tileIndex].transform.position = nextPos;
    }
}
