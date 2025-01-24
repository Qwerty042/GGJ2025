using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AmIClicked : MonoBehaviour, IPointerClickHandler
{
    public bool isClicked = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicking on the object");
        isClicked = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
