using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI bestValueText;
    public TextMeshProUGUI bestText;

    public GameObject GameOverPanel;
    public GameObject GameOverEffectPanel;

    public GameObject StartEffectPanel;

    public GameObject HowToPlayPanel;

    [HideInInspector]
    public bool isDead;


    int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance);
            Instance = this;
        }

        isDead = false;
        Application.targetFrameRate = 60;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        StartCoroutine(StartEffect());
    }


    IEnumerator StartEffect()
    {
        StartEffectPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        StartEffectPanel.SetActive(false);
        yield break;
    }


    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            bestValueText.text = score.ToString();
            PlayerPrefs.SetInt("BestScore", score);
        }
    }


    public void GameOver()
    {
        isDead = true;
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        Time.timeScale = 0.1f;
        GameOverEffectPanel.SetActive(true);
        scoreText.color = Color.white;
        bestText.color = Color.gray;
        bestValueText.color = Color.gray;

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0.02f;
        GameOverPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(2.5f);
        Time.timeScale = 0f;


        yield break;
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    public void OpenHowToPanel(){
        HowToPlayPanel.SetActive(true);
    } 
    
    public void CloseHowToPanel(){
         HowToPlayPanel.SetActive(false);
    }


}
