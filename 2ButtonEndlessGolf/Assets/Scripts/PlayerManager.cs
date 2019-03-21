using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Break Apart New Input Handling and actual Player Managing

public class PlayerManager : MonoBehaviour
{
    public static event Action NewPlayerAdded = delegate { };
    public static event Action PlayerWasRemoved = delegate { };

    public static PlayerManager Instance { get; private set; }

    [Serializable]
    class Player
    {
        private static int NEXT_PLAYER_ID = 0;

        [SerializeField] private int _ID;
        [SerializeField] private string _name;
        [SerializeField] private Color _color;
        [SerializeField] private PlayerController _controller;
        [SerializeField] private InputController _input;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private PlayerUI _ui;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public PlayerController Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }
        public InputController Input
        {
            get { return _input; }
            set { _input = value; }
        }
        public GameObject PGameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }
        public Color Color
        {
            get => _color;
            set
            {
                UI.SetColor(value);
                Controller.SetColor(value);
                _color = value;
            }
        }
        public PlayerUI UI { get => _ui; set => _ui = value; }
        public string Name { get => _name; set => _name = value; }

        public Player(Transform parent, KeyCode primaryKey, KeyCode secondaryKey, string name, Color color)
        {
            // Set the ID
            ID = NEXT_PLAYER_ID++;

            // Create the Players game object
            PGameObject = Instantiate(GameManager.Instance.playerPrefab, parent);

            // Check components; add if necessary
            Controller = PGameObject.GetComponent<PlayerController>();
            if (Controller == null)
            {
                Controller = PGameObject.AddComponent<PlayerController>();
            }
            Controller.Init(ID);

            // TODO get type of and add input controller accordingly
            Input = PGameObject.GetComponent<InputController>();
            if (Input == null)
            {
                Input = PGameObject.AddComponent<StandaloneInputController>();
            }
            Input.Init(primaryKey, secondaryKey, ID);

            var uiElement = Instantiate(GameManager.Instance.playerUIPrefab, GameManager.Instance.playerUIContainer);
            UI = uiElement.GetComponent<PlayerUI>();
            if (UI == null)
            {
                UI = uiElement.AddComponent<PlayerUI>();
                // TODO set ui references
            }
            UI.Init(name, primaryKey, secondaryKey);

            Color = color;
            PGameObject.SetActive(false);
        }

        public void SetKinematic(bool isKinematic)
        {
            Controller.Rb.isKinematic = isKinematic;
        }

        public void DestroyPlayer()
        {
            Destroy (UI);
            Destroy (PGameObject);
        }
    }

    [Header("Players")]
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private List<Color> colors = new List<Color>
    {
        GetRGBColor(10f, 223f, 222f),  // Blue p1
        GetRGBColor(252f, 121f, 17f),  // Orange p2
        GetRGBColor(50f, 205f, 51f),   // Green p3
        GetRGBColor(237f, 4f, 121f)   // Pink p4
    };
    [SerializeField] private int currentPlayerID;
    [SerializeField] private List<Player> players;
    [SerializeField] private int remaningRetries;

    [Header("Rotation")]
    [SerializeField] private Vector2 directionToGoal;
    [SerializeField] private bool flipAngle;

    [Header("Current Player")]
    [SerializeField] private Quaternion selectedAngle;
    [SerializeField] private float selectedForce;
    public int availableRetrys;

    [Header("Undo and Respawning")]
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private bool isUndoing;
    [SerializeField] private TurnState undoTargetState;

    [Header("Input and New Player")]
    [SerializeField] private bool waitingForNextInput;
    [SerializeField] private KeyCode newPlayer_primaryKey;
    [SerializeField] private KeyCode newPlayer_secondaryKey;
    [SerializeField] private Dictionary<KeyCode, Player> playerKeyBindings;

    public int CurrentPlayerID { get; private set; }
    Player GetCurrentPlayer() => GetPlayer(currentPlayerID);
    Player GetPlayer(int id) => players.Find(x => x.ID == currentPlayerID);
    private bool IsInputFromPlayer(int id, KeyCode key) => IsInputFromPlayer(GetPlayer(id), key);
    bool IsInputFromCurrentPlayer(KeyCode pressedKey) => IsInputFromPlayer(GetCurrentPlayer(), pressedKey);
    bool IsInputFromPlayer(Player player, KeyCode pressedKey) => (player.Input.PrimaryKey == pressedKey || player.Input.SecondaryKey == pressedKey);
    bool IsPlayerInput(KeyCode pressedKey) => playerKeyBindings.ContainsKey(pressedKey);
    bool IsPlayerOneInput(KeyCode pressedKey)
    {
        var player = playerKeyBindings[pressedKey];
        var playerIndex = players.FindIndex(x => x == player);
        return playerIndex == 0;
    }


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

    static Color GetRGBColor(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
        StopAllCoroutines();
    }

    private void Start()
    {
        players = new List<Player>();
        playerKeyBindings = new Dictionary<KeyCode, Player>();
    }
    private void Update()
    {
        // TODO Only allow in main menu / start screen
        if (GameManager.Instance.GameState == GameState.MainMenu)
        {
            CheckForNewPlayer();
        }
    }
    
    private void OnEnable()
    {
        InputController.Game.Primary += OnPrimaryGame;
        InputController.Game.Secondary += OnSecondaryGame;

        InputController.Menu.Primary += OnPrimaryMenu;
        InputController.Menu.Secondary += OnSecondaryMenu;

        GameManager.TurnStateChanged += OnTurnStateChanged;
        GameManager.GameStateChanged += OnGameStateChanged;
        GameManager.MapBoundsExit += OnMapBoundsExit;
    }
    private void OnDisable()
    {
        InputController.Game.Primary -= OnPrimaryGame;
        InputController.Game.Secondary -= OnSecondaryGame;
        
        InputController.Menu.Primary -= OnPrimaryMenu;
        InputController.Menu.Secondary -= OnSecondaryMenu;
        
        GameManager.TurnStateChanged -= OnTurnStateChanged;
        GameManager.GameStateChanged -= OnGameStateChanged;
        GameManager.MapBoundsExit -= OnMapBoundsExit;   
    }

    #region Input Listeners
    void OnPrimaryGame(KeyCode key)
    {
        if (!IsInputFromCurrentPlayer(key)) return;
        GameManager.Instance.AdvanceTurn(true);
    }

    void OnSecondaryGame(KeyCode key)
    {
        // Open Pause Menu
    }

    private void OnPrimaryMenu(KeyCode key)
    {
        Debug.Log("Primary Menu");
        MenuManager.Instance.SelectButton();
    }

    private void OnSecondaryMenu(KeyCode key)
    {
        var isPlayerOne = IsPlayerOneInput(key);
        var isCurrentPlayer = IsInputFromCurrentPlayer(key);

        switch (GameManager.Instance.GameState)
        {
            case GameState.MainMenu:
                RemovePlayer(playerKeyBindings[key].ID);
                break;
            case GameState.Paused:
                // Allow undo for current player
                if (isCurrentPlayer)
                {
                    StartUndo();
                }
                break;

            case GameState.LevelSelect:
            case GameState.GameOver:
                if (isPlayerOne)
                {
                    // Select stuff
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region Game Listeners
    private void OnGameStateChanged(GameState newState, GameState oldState)
    {
        switch (newState)
        {
            case GameState.MainMenu:

                break;
            case GameState.Playing:
                ActivatePlayerObjects();
                break;
            default:
                ActivatePlayerObjects(false);
                break;
        }
    }

    void OnTurnStateChanged(TurnState newState, TurnState oldState)
    {
        if (!isUndoing)
        {
            switch (oldState)
            {
                case TurnState.NotPlaying:
                    break;
                case TurnState.Start:
                    break;
                case TurnState.Angle:
                    SelectAngle();
                    break;
                case TurnState.Power:
                    SelectPower();
                    HitBall();
                    break;
                case TurnState.Firing:
                    break;
                case TurnState.End:
                    break;
                default:
                    break;
            }
        }

        switch (newState)
        {
            case TurnState.NotPlaying:
                break;
            case TurnState.Start:
                StartTurn();
                break;
            case TurnState.Angle:
                break;
            case TurnState.Power:
                SetUndoState(TurnState.Angle);
                break;
            case TurnState.Firing:
                StartCoroutine(WaitForMovementStop());
                break;
            case TurnState.End:
                SetUndoState(TurnState.Power);
                StartEndingTurn();
                break;
        }
    }
    #endregion

    #region Player Management
    private void CheckForNewPlayer()
    {
        var key = GetKeyInput();

        if (key != KeyCode.None)
        {
            if (players.Count >= maxPlayers)
            {
                print("Reached player limit " + players.Count);
                return;
            }

            if (!waitingForNextInput)
            {
                waitingForNextInput = true;
                newPlayer_primaryKey = key;
                GameManager.Instance.newPlayerDialog.ShowDialog($"P{players.Count + 1}", newPlayer_primaryKey.ToString(), colors[players.Count]);
            }
            else
            {
                newPlayer_secondaryKey = key;
                if (newPlayer_secondaryKey != KeyCode.None && newPlayer_secondaryKey != newPlayer_primaryKey)
                {
                    AddPlayer(newPlayer_primaryKey, newPlayer_secondaryKey);
                    waitingForNextInput = false;
                    newPlayer_primaryKey = KeyCode.None;
                    GameManager.Instance.newPlayerDialog.HideDialog();
                }
            }
        }
    }

    private KeyCode GetKeyInput()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                if (playerKeyBindings.ContainsKey(kcode))
                {
                    // Warn in new play dialog
                    if (newPlayer_primaryKey != KeyCode.None)
                    {
                        GameManager.Instance.newPlayerDialog.DisplayKeyInUse(kcode.ToString());
                    }

                    return KeyCode.None;
                }

                return kcode;
            }
        }
        return KeyCode.None;
    }

    void ActivatePlayerObjects(bool activate = true)
    {
        foreach (var player in players)
        {
            player.PGameObject.SetActive(activate);
        }
    }

    // TODO Create and associate controls
    private void AddPlayer(KeyCode primary, KeyCode secondary)
    {
        var newPlayer = new Player(transform, primary, secondary, $"P{players.Count + 1}", colors[players.Count]);
        players.Add(newPlayer);

        // Register Keybinds
        playerKeyBindings.Add(primary, newPlayer);
        playerKeyBindings.Add(secondary, newPlayer);

        UpdatePlayerPlaceholder();

        NewPlayerAdded();
    }

    private void UpdatePlayerPlaceholder()
    {
        var maxPlayersActive = players.Count >= maxPlayers;
        var placeholder = GameManager.Instance.pressToJoinPlaceholder;
        if (maxPlayersActive)
        {
            placeholder.SetActive(false);
        }
        else
        {
            placeholder.SetActive(true);
            placeholder.transform.SetAsLastSibling();
        }
    }

    public bool RemovePlayer(int id)
    {
        var player = players.Find(x => x.ID == id);
        if (player == null) return false;

        // Remove from key mapping lookup
        playerKeyBindings.Remove(player.Input.PrimaryKey);
        playerKeyBindings.Remove(player.Input.SecondaryKey);

        // Remove from players
        players.Remove(player);

        // Cleanup
        player.DestroyPlayer();

        UpdatePlayerPlaceholder();

        PlayerWasRemoved();
        return true;
    }
    #endregion

    void SelectAngle()
    {
        // See if the angle has to be flipped
        directionToGoal = GameManager.Instance.goalObject.transform.position - transform.position;
        flipAngle = directionToGoal.x > 0;

        selectedAngle = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 90f), GolfHitManager.Instance.CurrentAnglePercentage);
    }
    void SelectPower()
    {
        selectedForce = GolfHitManager.Instance.CurrentPowerPercentage * GameManager.Instance.force;
    }

    void HitBall()
    {
        // Quick access
        var player = GetCurrentPlayer();
        var pObject = player.PGameObject;
        var controller = player.Controller;
        
        // Remember the last position for undoing & out of bounds
        lastPosition = pObject.transform.position;

        var force = selectedAngle * new Vector2(selectedForce, 0);
        if (flipAngle)
        {
            //Debug.Log("Flipping angle...");
            force.x = -force.x;
        }
        //Debug.Log("Firing! " + force);
        Debug.DrawLine(pObject.transform.position, pObject.transform.position + force, Color.magenta, 5f);
        GetCurrentPlayer().SetKinematic(false);
        controller.Rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    IEnumerator WaitForMovementStop()
    {
        var player = GetCurrentPlayer();
        player.Controller.IsMoving = true;

        while (player.Controller.IsMoving)
        {
            //Debug.Log("Is moving");
            yield return new WaitForSeconds(GameManager.Instance.checkRate);
        }
        //Debug.Log("Finished moving");
        GetCurrentPlayer().SetKinematic(true);
        GameManager.Instance.AdvanceTurn();
        yield return null;
    }

    #region Undo
    void SetUndoState(TurnState state)
    {
        if (isUndoing) return;
        undoTargetState = state;
    }
    public void StartUndo()
    {
        isUndoing = true;
        StartCoroutine(Undo());
    }
    IEnumerator Undo()
    {
        var currentTurnState = GameManager.Instance.TurnState;
        var allowUndo = (currentTurnState == TurnState.End || currentTurnState == TurnState.Power);
        var player = GetCurrentPlayer();

        if (currentTurnState == TurnState.End)
        {
            if (player.Controller.availableUndos > 0)
            {
                // Interrupt
                StopCoroutine("SwitchTurn");
                GetCurrentPlayer().Controller.availableUndos--;
                //Debug.Log("Interrupting player switch... (interrupts remaining: " + GetCurrentPlayer().Controller.availableUndos + ")");
            }
            else
            {
                // tell the player that they don't have any more undos and update interface
                //Debug.LogWarning("No undos remaining for current player");
                allowUndo = false;
            }
        }

        if (allowUndo && GameManager.Instance.UndoMove(undoTargetState))
        {
            player.SetKinematic(true);
            //Debug.LogWarning("Cannot undo!");
        }

        yield return null;
        isUndoing = false;
    }
    private void OnMapBoundsExit(Collider2D collision)
    {
        var player = GetCurrentPlayer();
        if (collision.gameObject == player.PGameObject)
        {
            //Debug.Log("Out of bounds!");

            ResetPlayerPosition(player);

            if (remaningRetries > 0)
            {
                remaningRetries--;

                // Stop waiting for move finish
                StopCoroutine("WaitForMovementStop");

                // Return to angle select
                GolfHitManager.Instance.ResetValues();
                SetUndoState(TurnState.Power);
                StartUndo();
            }
            else
            {
                // Out of moves - End turn
                StartEndingTurn();
            }
        }
    }
    private void ResetPlayerPosition(Player targetPlayer)
    {
        targetPlayer.Controller.Rb.velocity = Vector2.zero;
        targetPlayer.PGameObject.transform.position = lastPosition;
    }
    #endregion

    #region Turn Setup
    void StartTurn()
    {
        //Debug.Log($"Starting Player {currentPlayerID}'s turn ...");
        SetupTurn();
        // TODO Add transition
        GameManager.Instance.AdvanceTurn();
    }
    void SetupTurn()
    {
        remaningRetries = GameManager.Instance.availableRetriesPerShot;
    }
    void StartEndingTurn()
    {
        var nextPlayerIndex = (players.FindIndex(x => x.ID == currentPlayerID) + 1) % players.Count;
        StartCoroutine(SwitchTurn(nextPlayerIndex));
    }
    IEnumerator SwitchTurn(int nextPlayerIndex)
    {
        Debug.Log($"Ending Player {currentPlayerID}'s turn ...");
        Debug.Log($"Switching players in {GameManager.Instance.endOfTurnTime} seconds ...");
        yield return new WaitForSeconds(GameManager.Instance.endOfTurnTime);
        currentPlayerID = players[nextPlayerIndex].ID;
        GameManager.Instance.AdvanceTurn();
    }
    #endregion
}
