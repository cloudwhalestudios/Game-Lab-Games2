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

    public static int GameCount
    {
        get { return _gameCount; }
        private set { _gameCount = value; }
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = -1;

    [Header("Accessibility")]
    public float autoInterval = 1f;
    public float endOfTurnTime = 2f;

    [Header("Current Level Config")]
    public GameObject goalObject;
    public float checkPointBelow;
    public GameObject mapBounds;

    [Header("Scene Management")]
    public Object mainMenuScene;
    public Object levelSelect;
    public List<LevelController> levelPrefabs;

    [Header("Player Config")]
    public GameObject playerPrefab;
    public RectTransform playerUIContainer;
    public GameObject playerUIPrefab;
    public GameObject pressToJoinPlaceholder;
    public NewPlayerDialog newPlayerDialog;
    public float force = 200;
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
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
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
        StartMainMenu();
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

        // StartUp Scene
        //SceneManager.LoadScene(mainMenuScene.name);
    }

    public void StartLevelSelect ()
    {
        GameState = GameState.LevelSelect;
        InputController.ActiveInputMode = InputController.InputMode.Menu;

        // LevelSelect Scene    
        SceneManager.LoadScene(levelSelect.name);
    }

    // A new game official starts
    public void StartGame(LevelController level)
    {
        GameState = GameState.Playing;
        InputController.ActiveInputMode = InputController.InputMode.Game;

        SceneManager.LoadScene(level.scene.name);

        if (SoundManager.Instance.backgroundGame != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundGame);
        }
        StartLevel();
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
