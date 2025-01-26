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
            int existingHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (GameManager.Score > existingHighScore)
            {
                PlayerPrefs.SetInt("HighScore", GameManager.Score);
                PlayerPrefs.Save();
            }

            finalScoreText.text = $"FINAL SCORE:\n{GameManager.Score}\n\nPLAYER BEST:\n{PlayerPrefs.GetInt("HighScore")}";
        }
        else if(GameManager.gameMode == GameManager.GameMode.gmSPEED)
        {
            float existingBestTime = PlayerPrefs.GetFloat("BestTime", 0f);
            if(GameManager.timeScore < existingBestTime)
            {
                PlayerPrefs.SetFloat("BestTime", GameManager.timeScore);
                PlayerPrefs.Save();
            }

            int minutes = Mathf.FloorToInt(GameManager.timeScore / 60f);
            float seconds = GameManager.timeScore - (minutes * 60f);
            if(minutes == 0)
                finalScoreText.text = $"FINAL TIME:\n{seconds:F2}s";
            else
                finalScoreText.text = $"FINAL TIME:\n{minutes}m{seconds,5:00.00}s";

            minutes = Mathf.FloorToInt(PlayerPrefs.GetFloat("BestTime") / 60f);
            seconds = PlayerPrefs.GetFloat("BestTime") - (minutes * 60f);
            if(minutes == 0)
                finalScoreText.text += $"\n\nPLAYER BEST TIME:\n{seconds:F2}s";
            else
                finalScoreText.text += $"\n\nPLAYER BEST TIME:\n{minutes}m{seconds,5:00.00}s";

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
