using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    private float timer;
    private int index = 0;
    private GameObject[] tiles = new GameObject[7]; 
    Vector3 spawnPos = new Vector3(-8f, 0f);

    int cubeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 0.1f && cubeCount < 7)
        {
            spawnPos.x += 2;
            tiles[cubeCount] = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
            cubeCount++;
            timer = 0;
            Debug.Log(tiles);
        }

        if(cubeCount >= 7)
        {
            tiles[index].transform.position += new Vector3(0,10f * Time.deltaTime,0);
        }
    }
}
