using SgLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public enum GameState
{
    MainMenu,
    LevelSelect,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public enum TurnState
{
    NotPlaying,
    Start,
    Angle,
    Power,
    Firing,
    End
}

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; }

    public static event Action<GameState, GameState> GameStateChanged = delegate { };
    public static event Action<TurnState, TurnState> TurnStateChanged = delegate { };
    public static event Action<Collider2D> MapBoundsExit = delegate { };

    private static bool isRestart;

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }

    public TurnState TurnState
    {
        get
        {
            return _turnState;
        }
        private set
        {
            if (value != _turnState)
            {
                TurnState oldState = _turnState;
                _turnState = value;

                TurnStateChanged(_turnState, oldState);
            }
        }
    }

    [SerializeField] private GameState _gameState;
    [SerializeField] private TurnState _turnState = TurnState.NotPlaying;
    
    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = -1;

    [Header("Accessibility")]
    public float autoInterval = 1f;
    public float endOfTurnTime = 2f;

    [Range(1, 10)] public int angleStepCount = 5;
    [Range(1, 10)] public int powerStepCount = 5;
    public float minPowerIndicatorScale = 1;
    public float maxPowerIndicatorScale = 2;


    [Header("Current Level Config")]
    public GameObject goalObject;
    public float checkPointBelow;
    public GameObject mapBounds;

    [Header("Player Config")]
    public GameObject playerPrefab;
    public RectTransform playerUIContainer;
    public GameObject playerUIPrefab;
    public GameObject pressToJoinPlaceholder;
    public NewPlayerDialog newPlayerDialog;

    [Header("Gameplay Config")]
    public float minForce = 20f;
    public float maxForce = 200f;

    public float minAngle = 10f;
    public float maxAngle = 80f;

    public float checkRate = 0.2f;
    public float checkGroundRadius = 0.1f;
    public LayerMask groundLayer;

    public float friction = 0.03f;
    public float bounciness = 0.2f;

    public int availableUndosPerLevel = 3;
    public int availableRetriesPerShot = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            _gameState = Instance.GameState;

            LoadInstance();
            DestroyImmediate(Instance);
            Instance = this;
        }
    }
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Initial setup
        Application.targetFrameRate = targetFrameRate;
    }

    public void LoadInstance()
    {
        switch (GameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.LevelSelect:
                break;
            case GameState.Playing:
                StartLevel();
                break;
            case GameState.Paused:
                break;
            case GameState.PreGameOver:
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }

    public void AdvanceTurn(bool playerInput = false)
    {
        switch (TurnState)
        {
            case TurnState.NotPlaying:
                if (!playerInput) TurnState = TurnState.Start;
                break;
            case TurnState.Start:
                if (!playerInput) TurnState = TurnState.Angle;
                break;
            case TurnState.Angle:
                TurnState = TurnState.Power;
                break;
            case TurnState.Power:
                TurnState = TurnState.Firing;
                break;
            case TurnState.Firing:
                if (!playerInput) TurnState = TurnState.End;
                break;
            case TurnState.End:
                if (!playerInput) TurnState = TurnState.Start;
                break;
            default:
                break;
        }
    }

    public bool UndoMove(TurnState targetState)
    {
        switch (targetState)
        {
            case TurnState.Angle:
            case TurnState.Power:
                TurnState = targetState;
                return true;
            default:
                return false;
        }
    }

    // Make initial setup and preparations before the game can be played
    public void StartMainMenu()
    {
        GameState = GameState.MainMenu;
        InputController.ActiveInputMode = InputController.InputMode.Menu;

        if (SoundManager.Instance.backgroundMenu != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundMenu);
        }

        // StartUp Scene
        LevelManager.Instance.LoadMainMenu();
    }

    public void StartLevelSelect ()
    {
        GameState = GameState.LevelSelect;
        InputController.ActiveInputMode = InputController.InputMode.Menu;

        if (SoundManager.Instance.backgroundMenu != null && !SoundManager.Instance.bgmSource.isPlaying)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundMenu);
        }

        LevelManager.Instance.LoadLevelSelect();
    }

    // A new game official starts
    public void StartGame(int levelIndex)
    {
        GameState = GameState.Playing;
        InputController.ActiveInputMode = InputController.InputMode.Game;

        if (SoundManager.Instance.backgroundGame != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundGame);
        }

        //LevelManager.Instance.LoadLevel(levelIndex);
    }

    public void StartLevel()
    {
        TurnState = TurnState.Start;
        
    }

    // Called when the player died
    public void GameOver()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.win, true);
        GameState = GameState.GameOver;
    }

    // Start a new game
    public void RestartGame(float delay = 0)
    {
        isRestart = true;
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MapBoundsExit(collision);
    }

    public void Quit()
    {
        // Exit Game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("google.com");
#else
        Application.Quit();
#endif
    }
}
