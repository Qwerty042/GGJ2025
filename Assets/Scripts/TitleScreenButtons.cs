using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreenButtons : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public AudioSource audio;
    public Color hoverColor;
    private Color originalColor;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        Debug.Log("Working...");

    }

    public void OnMouseOver()
    {
        if (spriteRenderer.color == originalColor)
        {
            audio.Play();
        }
        // Change color to the hover color when mouse enters
        spriteRenderer.color = hoverColor;

    }

    public void OnMouseExit()
    {
        // Revert back to original color when mouse exits
        spriteRenderer.color = originalColor;
    }

    public void OnMouseDown()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
