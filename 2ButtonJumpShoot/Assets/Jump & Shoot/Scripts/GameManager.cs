using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using AccessibilityInputSystem.TwoButtons;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI bestValueText;
    public TextMeshProUGUI bestText;

    [Space]
    public GameObject GameOverPanel;
    public GameObject GameOverButtonsParent;
    public GameObject PauseMenuPanel;
    public GameObject PauseMenuButtonsParent;
    public GameMenuController menuController;

    [Space]
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
        UpdateTopScoreText();

        scoreText.text = "0";

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

        SetTopScore(score);
    }

    public void SetTopScore(int score)
    {
        if (score > UserProgress.Current.TopScore)
        {
            UserProgress.Current.TopScore = score;
            UpdateTopScoreText();

            AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Highscore);
        }
    }

    void UpdateTopScoreText()
    {
        bestValueText.text = UserProgress.Current.TopScore.ToString();
    }

    public void GameOver()
    {
        isDead = true;

        GameOverEffectPanel.SetActive(true);
        scoreText.color = Color.white;
        bestText.color = Color.gray;
        bestValueText.color = Color.gray;

        SetGameOverActive();

        TimeScaleController.Instance.Pause(true);
    }

    public void Pause()
    {
        TimeScaleController.Instance.Pause();

        scoreText.color = Color.white;
        bestText.color = Color.gray;
        bestValueText.color = Color.gray;

        SetPauseMenuActive();
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_cancel);
    }
    public void Resume()
    {
        TimeScaleController.Instance.Pause(false);

        var color = new Color(0, 0, 0, 29f / 255);
        scoreText.color = color;
        bestText.color = color;
        bestValueText.color = color;

        SetPauseMenuActive(false);
    }

    void SetPauseMenuActive(bool activate = true)
    {
        menuController.SetMenu(PauseMenuPanel, PauseMenuButtonsParent);
        GameOverPanel.SetActive(false);

        if (activate) MenuManager.Instance.ShowMenu();
        else MenuManager.Instance.HideMenu();
    }

    void SetGameOverActive(bool activate = true)
    {
        menuController.SetMenu(GameOverPanel, GameOverButtonsParent);
        PauseMenuPanel.SetActive(false);

        if (activate) MenuManager.Instance.ShowMenu();
        else MenuManager.Instance.HideMenu();
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
