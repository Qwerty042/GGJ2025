using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{

    public float fadeSpeed = 2.0f;
    public TextMeshProUGUI textMesh;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalColor = textMesh.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (textMesh != null)
        {
            // Reduce the alpha value over time
            float newAlpha = Mathf.Max(originalColor.a - fadeSpeed * Time.deltaTime, 0);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            // Update the original color with the new alpha
            originalColor = textMesh.color;

            // Destroy the GameObject when alpha reaches 0
            if (newAlpha <= 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
