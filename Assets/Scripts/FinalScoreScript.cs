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
        if(GameManager.gameMode == GameManager.GameMode.gmSKILL)
        {
            finalScoreText.text = $"FINAL SCORE\n\n{GameManager.Score}";
        }
        else if(GameManager.gameMode == GameManager.GameMode.gmSPEED)
        {
            int minutes = Mathf.FloorToInt(GameManager.timeScore / 60f);
            float seconds = GameManager.timeScore - (minutes * 60f);
            if(minutes == 0)
                finalScoreText.text = $"FINAL TIME\n\n{seconds:F2}s";
            else
                finalScoreText.text = $"FINAL TIME\n\n{minutes}m{seconds,5:00.00}s";
        }
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
