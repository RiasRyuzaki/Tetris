using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;

    void Start()
    {
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void MainMenuB()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}
