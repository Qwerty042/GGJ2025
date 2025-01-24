using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileswapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if(hit.transform != null)
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
}
