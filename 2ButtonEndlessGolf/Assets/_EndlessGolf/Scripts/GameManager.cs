using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using SgLib;

public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };

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

    private GameState _gameState = GameState.Prepare;

    public static int GameCount
    {
        get { return _gameCount; }
        private set { _gameCount = value; }
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = 30;

    // List of public variable for gameplay tweaking

    [Header("Ground Config")]
    [HideInInspector]
    public int len = 10000;
    [HideInInspector]
    public float width = 100;
    public int levelComplicated = 10;
    public float minSlope = -5;
    public float maxSlope = 5;

    public float holeWidth = 2;
    public float holeHeight = 3;
    [Range(3, 10)]
    public int complicatedOfHole = 5;
    [Range(1, 7)]
    public float startPointSize = 5;

    public Material spriteMaterial;
    public GameObject holeCheckPoint;
    public float checkPointBelow;

    [Header("Player Config")]
    public Color isNotMoving;
    public Color isMoving;
    public float force = 200;
    public float checkRate = 0.2f;

    public GameObject dragArrow;
    public GameObject targetArrow;
    public GameObject inviArrow;
    public GameObject targetInArrow;

    public Transform checkGround;
    public float checkGroundRadius = 0.1f;
    public LayerMask groundLayer;

    public float friction = 0.03f;
    public float bounciness = 0.2f;

    [Header("Gameplay Config")]
    public int numberOfStroke = 5;
    public int strokeAdd = 3;

    public float minWindForce;
    public float maxWindForce;

    public float bodyXscale = 0.2f;
    public float bodyYscale = 0.2f;
    [HideInInspector]
    public float windForce;
    [Range(0f, 1f)]
    [HideInInspector]
    public float coinFrequency = 0.1f;

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

    void OnEnable()
    {
        PlayerController.PlayerDied += PlayerController_PlayerDied;
    }

    void OnDisable()
    {
        PlayerController.PlayerDied -= PlayerController_PlayerDied;
    }

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
        ScoreManager.Instance.Reset();

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

    // Listens to the event when player dies and call GameOver
    void PlayerController_PlayerDied()
    {
        GameOver();
    }

    // Make initial setup and preparations before the game can be played
    public void PrepareGame()
    {
        GameState = GameState.Prepare;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
        dragArrow.SetActive(false);
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

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
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
        dragArrow.SetActive(false);
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
