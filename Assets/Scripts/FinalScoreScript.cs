using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class script : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        finalScoreText.text = $"FINAL SCORE\n\n{GameManager.Score}";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            // Load the "Title Screen" scene
            SceneManager.LoadScene("Title Screen");
        }
    }
}
