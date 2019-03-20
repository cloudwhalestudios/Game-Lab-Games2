using SgLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Prepare,
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

    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };
    public static event System.Action<TurnState, TurnState> TurnStateChanged = delegate { };

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

    [SerializeField] private GameState _gameState = GameState.Prepare;

    [SerializeField] private TurnState _turnState = TurnState.NotPlaying;

    public static int GameCount
    {
        get { return _gameCount; }
        private set { _gameCount = value; }
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = 30;

    [Header("Accessibility")]
    public float autoInterval = 1f;

    [Header("Goal Config")]
    public GameObject goalObject;
    public float checkPointBelow;

    [Header("Player Config")]
    public GameObject playerPrefab;
    public Color isNotMoving;
    public Color isMoving;
    public float force = 200;
    public float checkRate = 0.2f;

    public float checkGroundRadius = 0.1f;
    public LayerMask groundLayer;

    public float friction = 0.03f;
    public float bounciness = 0.2f;

    public int availableUndosPerLevel = 3;

    [Header("Gameplay Config")]
    public float endOfTurnTime = 2f;

    public float minWindForce;
    public float maxWindForce;

    public float bodyXscale = 0.2f;
    public float bodyYscale = 0.2f;
    [HideInInspector]
    public float windForce;

    [Header("Camera Config")]
    public float smoothTime = 2f;
    public float distanceBottoCam = 15;
    [HideInInspector]
    public bool niceHit;

    [Header("Falling Effect")]
    public float minFallingSpeed = -20;
    public float maxFallingSpeed = 5;
    public float ratioWithWindForce = 1;

    // List of public variables referencing other objects
    [Header("Object References")]
    public PlayerController playerController;
    public ParticleSystem fallingEffect;

    [HideInInspector]
    public ParticleSystem.VelocityOverLifetimeModule velocity;

    private float curWindForce;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
        velocity = fallingEffect.velocityOverLifetime;
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

        PrepareGame();
        velocity.y = new ParticleSystem.MinMaxCurve(minFallingSpeed, maxFallingSpeed);
    }

    void Update()
    {
        UpdateEffect();
    }

    public void UpdateEffect()
    {
        if (curWindForce != windForce)
        {
            float value = -(windForce * ratioWithWindForce);
            if (value == 0)
                velocity.y = new ParticleSystem.MinMaxCurve(minFallingSpeed, minFallingSpeed / 2);
            else
                velocity.y = new ParticleSystem.MinMaxCurve(minFallingSpeed, maxFallingSpeed);
            velocity.x = new ParticleSystem.MinMaxCurve(value, value);
            curWindForce = windForce;
        }
    }

    public void AdvanceTurn()
    {
        switch (TurnState)
        {
            case TurnState.NotPlaying:
                TurnState = TurnState.Start;
                break;
            case TurnState.Start:
                TurnState = TurnState.Angle;
                break;
            case TurnState.Angle:
                TurnState = TurnState.Power;
                break;
            case TurnState.Power:
                TurnState = TurnState.Firing;
                break;
            case TurnState.Firing:
                TurnState = TurnState.End;
                break;
            case TurnState.End:
                TurnState = TurnState.Start;
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

    // Listens to the event when player dies and call GameOver
    void PlayerController_PlayerDied()
    {
        GameOver();
    }

    // Make initial setup and preparations before the game can be played
    public void PrepareGame()
    {
        GameState = GameState.Prepare;

        if (isRestart)
        {
            isRestart = false;
            StartGame();
        }
    }

    // A new game official starts
    public void StartGame()
    {
        GameState = GameState.Playing;
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
        }
    }

    // Called when the player died
    public void GameOver()
    {
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.StopMusic();
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver, true);
        GameState = GameState.GameOver;
        GameCount++;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;

        // Add other game over actions here if necessary
    }

    // Start a new game
    public void RestartGame(float delay = 0)
    {
        isRestart = true;
        _gameCount++;
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
