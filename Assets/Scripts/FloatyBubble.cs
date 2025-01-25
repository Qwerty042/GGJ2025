using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyBubble : MonoBehaviour
{
    public float despawnHeight = 10f;

    static System.Random rand = new System.Random();
    private Vector3 startPos;
    private float speed;
    private float size;

    // Start is called before the first frame update
    void Start()
    {
        speed = rand.Next(10, 200) / 100f;
        size = rand.Next(10, 100) / 100f;

        startPos = new Vector3(rand.Next(-950, 950) / 100f,
                                       rand.Next(-800, -600) / 100f);
        gameObject.transform.position = startPos;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = rand.Next(100) >= 50 ? 1 : -1;

        Vector3 randScale = new Vector3(size/5f, size/5f, size/5f);
        gameObject.transform.localScale = randScale;
        // Debug.Log($"ScaleX:{gameObject.transform.localScale.x} ScaleY:{gameObject.transform.localScale.y}");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += Vector3.up * Time.deltaTime;
        Vector3 nextPos = gameObject.transform.position;
        nextPos.y += speed * Time.deltaTime;
        nextPos.x = (1/size) * Mathf.Sin(size*nextPos.y) + startPos.x;
        gameObject.transform.position = nextPos;

        if(transform.position.y >= despawnHeight)
        {
            Destroy(gameObject);
        }
    }
}
