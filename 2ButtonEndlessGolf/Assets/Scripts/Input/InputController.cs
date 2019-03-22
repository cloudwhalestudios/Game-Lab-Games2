using System;
using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    public enum InputMode
    {
        None,
        Game,
        Menu,
        Other
    }

    public class ModeControls
    {
        public InputMode TargetMode;
        public event Action<KeyCode> Primary;
        public event Action<KeyCode> Secondary;

        public ModeControls(InputMode targetMode)
        {
            TargetMode = targetMode;
        }

        public void LogHandles(string prefix)
        {
            print(prefix + " Handles: Primary has handles:" + (Primary != null).ToString() + " | Secondary has handles:" + (Secondary != null).ToString());
        }

        public void InvokePrimary(KeyCode pressedKey)
        {
            Primary(pressedKey);
        }
        public void InvokeSecondary(KeyCode pressedKey)
        {
            Secondary(pressedKey);
        }
    }

    private static InputMode _activeInputMode;
    public static InputMode ActiveInputMode
    {
        get
        {
            return _activeInputMode;
        }

        set
        {
            if (value != _activeInputMode)
            {
                switch (value)
                {
                    case InputMode.Game:
                        ActiveControls = Game;
                        break;
                    case InputMode.Menu:
                        ActiveControls = Menu;
                        break;
                    case InputMode.Other:
                        ActiveControls = Other;
                        break;
                    default:
                        ActiveControls = null;
                        break;
                }
                Debug.Log("Setting Mode to " + value.ToString());
                _activeInputMode = value;
            }
        }
    }

    public int PlayerID
    {
        protected set => _playerID = value;
        get => _playerID;
    }
    public KeyCode PrimaryKey
    {
        get => primaryKey;
        protected set => primaryKey = value;
    }
    public KeyCode SecondaryKey
    {
        get => secondaryKey;
        protected set => secondaryKey = value;
    }

    [SerializeField, ReadOnly] protected int _playerID = -1;
    [SerializeField, ReadOnly] private KeyCode primaryKey;
    [SerializeField, ReadOnly] private KeyCode secondaryKey;

    public static ModeControls Game     = new ModeControls(InputMode.Game);
    public static ModeControls Menu     = new ModeControls(InputMode.Menu);
    public static ModeControls Other    = new ModeControls(InputMode.Other);

    protected static ModeControls ActiveControls;

    public abstract void Init(KeyCode primary, KeyCode secondary, int id);
    protected static void OnPrimary(KeyCode pressedKey)
    {
        //Debug.Log("Primary");
        ActiveControls.InvokePrimary(pressedKey);
    }

    protected static void OnSecondary(KeyCode pressedKey)
    {
        //Debug.Log("Secondary");
        ActiveControls.InvokeSecondary(pressedKey);
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(PrimaryKey))
            OnPrimary(PrimaryKey);

        if (Input.GetKeyDown(SecondaryKey))
            OnSecondary(SecondaryKey);
    }
}