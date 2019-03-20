using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Serializable]
    class Player
    {
        private static int NEXT_PLAYER_ID = 0;

        [SerializeField] private int _ID;
        [SerializeField] private PlayerController _controller;
        [SerializeField] private InputController _input;
        [SerializeField] private GameObject _gameObject;

        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }
        public PlayerController Controller
        {
            get
            {
                return _controller;
            }

            set
            {
                _controller = value;
            }
        }
        public InputController Input
        {
            get
            {
                return _input;
            }

            set
            {
                _input = value;
            }
        }

        public GameObject PGameObject
        {
            get
            {
                return _gameObject;
            }

            set
            {
                _gameObject = value;
            }
        }

        public Player(Transform parent, KeyCode primaryKey, KeyCode secondaryKey)
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

            PGameObject.SetActive(false);
        }

        public void DestroyPlayer()
        {
            Destroy (PGameObject);
        }
    }

    public int CurrentPlayerID
    {
        get
        {
            return currentPlayerID;
        }

        private set
        {
            currentPlayerID = value;
        }
    }

    [Header("Current Turn Information (do not touch)")]
    [SerializeField] private int currentPlayerID;
    [SerializeField] private List<Player> players;
    private Vector3 lastPosition;
    [SerializeField] private Quaternion selectedAngle;
    [SerializeField] private float selectedForce;
    [SerializeField] private bool flipAngle;
    private Vector2 directionToGoal;

    private bool isUndoing;
    private TurnState undoTargetState;

    private bool waitingForNextInput;
    private KeyCode newPlayer_primaryKey;
    private Dictionary<KeyCode, Player> playerKeyBindings;

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
    }

    private void Start()
    {
        players = new List<Player>();
        playerKeyBindings = new Dictionary<KeyCode, Player>();
    }

    private void Update()
    {
        // TODO Only allow in main menu / start screen
        if (GameManager.Instance.GameState == GameState.Prepare)
        {
            if (waitingForNextInput)
            {
                var newPlayer_secondaryKey = GetKeyInput();
                if (newPlayer_secondaryKey != KeyCode.None && newPlayer_secondaryKey != newPlayer_primaryKey)
                {
                    AddPlayer(newPlayer_primaryKey, newPlayer_secondaryKey);
                    waitingForNextInput = false;
                }
                return;
            }
            CheckForNewPlayer();
        }
    }
    private void OnEnable()
    {
        InputController.Game.Primary += OnPrimary;
        InputController.Game.Secondary += OnSecondary;
        GameManager.TurnStateChanged += OnTurnStateChanged;
        GameManager.GameStateChanged += OnGameStateChanged;
    }
    private void OnDisable()
    {
        InputController.Game.Primary -= OnPrimary;
        InputController.Game.Secondary -= OnSecondary;
        GameManager.TurnStateChanged -= OnTurnStateChanged;
        GameManager.GameStateChanged -= OnGameStateChanged;
    }
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnGameStateChanged(GameState newState, GameState oldState)
    {
        switch (newState)
        {
            case GameState.Playing:
                InputController.ActiveInputMode = InputController.InputMode.Game;
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
                break;
            case TurnState.Firing:
                break;
            case TurnState.Angle:
                break;
            case TurnState.Power:
                SetUndoState(TurnState.Angle);
                break;
            case TurnState.End:
                SetUndoState(TurnState.Power);
                break;
        }
    }

    void OnPrimary(KeyCode keycode)
    {
        if (!IsInputFromCurrentPlayer(keycode)) return;
        GameManager.Instance.AdvanceTurn();
    }

    void OnSecondary(KeyCode keycode)
    {
        // Open Pause Menu
        Undo();
    }

    private void CheckForNewPlayer()
    {
        var key = GetKeyInput();
        if (key != KeyCode.None)
        {
            waitingForNextInput = true;
            newPlayer_primaryKey = key;
        }
    }

    private KeyCode GetKeyInput()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (playerKeyBindings.ContainsKey(kcode)) return KeyCode.None;
            if (Input.GetKeyDown(kcode))
                return kcode;
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
    public void AddPlayer(KeyCode primary, KeyCode secondary)
    {
        var newPlayer = new Player(transform, primary, secondary);
        players.Add(newPlayer);
        playerKeyBindings.Add(primary, newPlayer);
        playerKeyBindings.Add(secondary, newPlayer);
    }

    // TODO Destroy controller and input module
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
        return true;
    }

    Player GetCurrentPlayer()
    {
        return GetPlayer(currentPlayerID);
    }

    Player GetPlayer(int id)
    {
        return players.Find(x => x.ID == currentPlayerID);
    }

    bool IsInputFromCurrentPlayer(KeyCode pressedKey)
    {
        return IsInputFromPlayer(GetCurrentPlayer(), pressedKey);
    }

    bool IsInputFromPlayer(Player player, KeyCode pressedKey)
    {
        return (player.Input.PrimaryKey == pressedKey || player.Input.SecondaryKey == pressedKey);
    }

    void SelectPower()
    {
        selectedForce = GolfHitManager.Instance.CurrentPowerPercentage * GameManager.Instance.force;
    }

    void SelectAngle()
    {
        selectedAngle = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 90f), GolfHitManager.Instance.CurrentAnglePercentage);
    }

    void SetUndoState(TurnState state)
    {
        if (isUndoing) return;
        undoTargetState = state;
    }

    void Undo()
    {
        isUndoing = true;
        if(GameManager.Instance.TurnState == TurnState.End)
        {
            if (GetCurrentPlayer().Controller.availableUndos > 0)
            {
                StopCoroutine("SwitchTurn");
                GetCurrentPlayer().Controller.availableUndos--; 
                Debug.Log("Interrupting player switch... (interrupts remaining: " + GetCurrentPlayer().Controller.availableUndos + ")");
            }
            else
            {
                // tell the player that they don't have any more undos and update interface
                Debug.LogWarning("No undos remaining for current player");
                return;
            }
        }
        if(!GameManager.Instance.UndoMove(undoTargetState))
        {
            Debug.LogWarning("Cannot undo!");
        }
    }

    void SetupTurn()
    {
        directionToGoal = GameManager.Instance.goalObject.transform.position - transform.position;
        if (directionToGoal.x < 0)
        {
            flipAngle = true;
        }
        else
        {
            flipAngle = false;
        }
    }

    void EndTurn()
    {
        var nextPlayerIndex = players.FindIndex(x => x.ID == currentPlayerID) % players.Count;
        StartCoroutine(SwitchTurn(nextPlayerIndex));
    }

    IEnumerator SwitchTurn(int nextPlayerIndex)
    {
        yield return new WaitForSeconds(GameManager.Instance.endOfTurnTime);
        currentPlayerID = players[nextPlayerIndex].ID;
        GameManager.Instance.AdvanceTurn();
    }
}
