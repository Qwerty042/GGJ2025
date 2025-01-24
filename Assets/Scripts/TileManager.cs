using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject tileSelectBoarderPrefab;
    public Vector3[] tileSpots;// = new Vector3[7];

    // private enum GameStateEnum
    // {
    //     gsSELECTING,
    //     gsSWAPPING
    // }

    private GameObject[] tiles;
    private GameObject[] tileSelectBoarders = new GameObject[2] {null, null};
    private GameObject[] swapTiles = new GameObject[2] {null, null};
    private bool swapping = false;
    private int NUM_TILES;
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
                ClearTileSelection();
            }
            else if(hit.transform.name.StartsWith(tilePrefab.name))
            {
                if(swapTiles[0] == null) // No other tile selected
                {
                    swapTiles[0] = hit.transform.gameObject;
                    //TODO: Instantiate selection boarder
                    Debug.LogFormat("Selected Tile! Name:{0} ID:{1}", swapTiles[0].name, swapTiles[0].GetInstanceID().ToString());
                }
                else if (swapTiles[0].GetInstanceID() == hit.transform.gameObject.GetInstanceID()) // Tile is already selected, deselect:
                {
                    ClearTileSelection();
                }
                else
                {
                    swapTiles[1] = hit.transform.gameObject;
                    Debug.LogFormat("Swap Name_0:{0},ID_0:{1} with Name_1:{2},ID_1:{3}", swapTiles[0].name, 
                                                                                         swapTiles[0].GetInstanceID().ToString(),
                                                                                         swapTiles[1].name,
                                                                                         swapTiles[1].GetInstanceID().ToString());
                    swapping = true;
                }
            }
        }
    }

    void ClearTileSelection()
    {
        Debug.Log("Clear selected");
        swapTiles = new GameObject[2] {null, null};
        //TODO: clear selection boarders
    }
}
