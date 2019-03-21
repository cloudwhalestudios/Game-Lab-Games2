using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject header;
    public GameObject title;
    public Text score;
    public Text bestScore;
    public Text coinText;
    public Text strokeRemainingText;
    public GameObject strokeBox;
    public GameObject windBox;
    public GameObject newBestScore;
    public GameObject playBtn;
    public GameObject restartBtn;
    public GameObject menuButtons;
    public GameObject settingsUI;
    public GameObject soundOnBtn;
    public GameObject soundOffBtn;
    public GameObject musicOnBtn;
    public GameObject musicOffBtn;

    Animator scoreAnimator;
    private PlayerController playerController;

    void OnEnable()
    {
        GameManager.GameStateChanged    += GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated       += OnScoreUpdated;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged    -= GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated       -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        //playerController = GameManager.Instance.playerController;

        Reset();
        ShowStartUI();
    }

    // Update is called once per frame
    void Update()
    {
        //bestScore.text = ScoreManager.Instance.HighScore.ToString();
        //coinText.text = CoinManager.Instance.Coins.ToString();

        if (settingsUI.activeSelf)
        {
            UpdateSoundButtons();
            UpdateMusicButtons();
        }
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {              
            ShowGameUI();
        }
        else if (newState == GameState.PreGameOver)
        {
            // Before game over, i.e. game potentially will be recovered
        }
        else if (newState == GameState.GameOver)
        {
            Invoke("ShowGameOverUI", 1f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    void Reset()
    {
        ButtonClickSound();
        //mainCanvas.SetActive(true);
        //characterSelectionUI.SetActive(false);
        header.SetActive(false);
        title.SetActive(false);
        score.gameObject.SetActive(false);
        newBestScore.SetActive(false);
        playBtn.SetActive(false);
        menuButtons.SetActive(false);
        strokeBox.SetActive(false);
        windBox.SetActive(false);

        // Hidden by default
        settingsUI.SetActive(false);
    }

    public void StartGame()
    {
        ButtonClickSound();
        GameManager.Instance.StartGame();
    }

    public void EndGame()
    {
        ButtonClickSound();
        GameManager.Instance.GameOver();
    }

    public void RestartGame()
    {
        ButtonClickSound();
        GameManager.Instance.RestartGame(0.2f);
    }

    public void ShowStartUI()
    {
        settingsUI.SetActive(false);

        header.SetActive(true);
        title.SetActive(true);
        playBtn.SetActive(true);
        restartBtn.SetActive(false);
        menuButtons.SetActive(true);
    }

    public void ShowGameUI()
    {
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        playBtn.SetActive(false);
        menuButtons.SetActive(false);
        strokeBox.SetActive(true);
        windBox.SetActive(true);
    }

    public void ShowGameOverUI()
    {
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        newBestScore.SetActive(ScoreManager.Instance.HasNewHighScore);
        strokeBox.SetActive(false);
        windBox.SetActive(false);

        playBtn.SetActive(false);
        restartBtn.SetActive(true);
        menuButtons.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void ShowSettingsUI()
    {
        ButtonClickSound();
        settingsUI.SetActive(true);
    }

    public void HideSettingsUI()
    {
        ButtonClickSound();
        settingsUI.SetActive(false);
    }

    public void ToggleSound()
    {
        ButtonClickSound();
        SoundManager.Instance.ToggleSound();
    }

    public void ToggleMusic()
    {
        ButtonClickSound();
        SoundManager.Instance.ToggleMusic();
    }

    public void ButtonClickSound()
    {
        Utilities.ButtonClickSound();
    }

    void UpdateSoundButtons()
    {
        if (SoundManager.Instance.IsSoundOff())
        {
            soundOnBtn.gameObject.SetActive(false);
            soundOffBtn.gameObject.SetActive(true);
        }
        else
        {
            soundOnBtn.gameObject.SetActive(true);
            soundOffBtn.gameObject.SetActive(false);
        }
    }

    void UpdateMusicButtons()
    {
        if (SoundManager.Instance.IsMusicOff())
        {
            musicOffBtn.gameObject.SetActive(true);
            musicOnBtn.gameObject.SetActive(false);
        }
        else
        {
            musicOffBtn.gameObject.SetActive(false);
            musicOnBtn.gameObject.SetActive(true);
        }
    }
}
