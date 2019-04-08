using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    int score = 0;
    public TextMeshProUGUI CurrentScoreTextTMPro;
    public TextMeshProUGUI BestScoreTextTMPro;

    public GameObject GameOverPanel;
    public GameObject GameOverEffectPanel;

    public GameObject touchToMoveTextObj;

    public GameObject StartFadeInObj;

    static int PlayCount;

    public bool GameOverMenuRestartActive;
    public bool GameOverMenuMainMenuActive;

    void Awake()
    {
        Application.targetFrameRate = 60;


        Time.timeScale = 1.0f;


        BestScoreTextTMPro.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        StartCoroutine(FadeIn());

    }

    void Update()
    {
        if (touchToMoveTextObj.activeSelf == false) return;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            touchToMoveTextObj.SetActive(false);
        }
        if (GameOverPanel.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Restart();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SceneManager.LoadScene("MainMenuSceneWave");
            }
        }
    }

    IEnumerator FadeIn()
    {
        StartFadeInObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartFadeInObj.SetActive(false);
        yield break;
    }

    public void addScore()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Score);
        score++;
        CurrentScoreTextTMPro.text = score.ToString();

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", score);
            BestScoreTextTMPro.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
            AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Highscore);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Gameover()
    {
        StartCoroutine(GameoverCoroutine());
    }


    IEnumerator GameoverCoroutine()
    {
        GameOverEffectPanel.SetActive(true);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.5f);
        GameOverPanel.SetActive(true);
        yield break;
    }
}
