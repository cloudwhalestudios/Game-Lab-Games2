using System;
using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    public enum InputMode
    {
        Game,
        Menu,
        Other
    }

    public class ModeControls
    {
        public InputMode TargetMode;
        public event Action<KeyCode> Primary = delegate { };
        public event Action<KeyCode> Secondary = delegate { };

        public ModeControls(InputMode targetMode)
        {
            TargetMode = targetMode;
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

    private static InputMode activeInputMode;
    public static InputMode ActiveInputMode
    {
        get
        {
            return activeInputMode;
        }

        set
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
                    Debug.LogError("Unknown active mode!");
                    break;
            }
            Debug.Log("Setting Mode to " + value.ToString());
            activeInputMode = value;
        }
    }

    public KeyCode PrimaryKey
    {
        get
        {
            return primaryKey;
        }

        protected set
        {
            primaryKey = value;
        }
    }

    public KeyCode SecondaryKey
    {
        get
        {
            return secondaryKey;
        }

        protected set
        {
            secondaryKey = value;
        }
    }

    [SerializeField] protected int _playerID = -1;
    public int PlayerID
    {
        protected set
        {
            _playerID = value;
        }
        get
        {
            return _playerID;
        }
    }
    [SerializeField] private KeyCode primaryKey;
    [SerializeField] private KeyCode secondaryKey;

    public static ModeControls Game        = new ModeControls(InputMode.Game);
    public static ModeControls Menu       = new ModeControls(InputMode.Menu);
    public static ModeControls Other       = new ModeControls(InputMode.Other);

    private static ModeControls ActiveControls;

    public abstract void Init(KeyCode primary, KeyCode secondary, int id);
    protected static void OnPrimary(KeyCode pressedKey)
    {
        ActiveControls.InvokePrimary(pressedKey);
    }

    protected static void OnSecondary(KeyCode pressedKey)
    {
        ActiveControls.InvokeSecondary(pressedKey);
    }
}