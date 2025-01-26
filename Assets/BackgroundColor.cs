using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.gameMode == GameManager.GameMode.gmSKILL)
        {
            mainCamera.backgroundColor = new Color32(82, 153, 154, 255);
        }
        else if (GameManager.gameMode == GameManager.GameMode.gmSPEED)
        {
            mainCamera.backgroundColor = new Color32(154, 82, 83, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
