using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    public GameObject bubblePrefab;
    private static System.Random rand = new System.Random();
    private float timer;
    private float timeToSpawn = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= timeToSpawn)
        {
            timer = 0;
            int spawnNumber = rand.Next(1, 3);
            for(int i = 0; i < spawnNumber; i++)
            {
                Instantiate(bubblePrefab);
            }
            timeToSpawn = rand.Next(20, 100) / 100f;
        }
    }
}
