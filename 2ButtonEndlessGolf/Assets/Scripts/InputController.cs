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
        public event Action Primary;
        public event Action Secondary;

        public ModeControls(InputMode targetMode)
        {
            TargetMode = targetMode;
        }

        public void InvokePrimary()
        {
            if (Primary != null)
                Primary.Invoke();
        }
        public void InvokeSecondary()
        {
            if (Secondary != null)
                Secondary.Invoke();
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
                    ActiveControls = Pause;
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


    public static ModeControls Game        = new ModeControls(InputMode.Game);
    public static ModeControls Pause       = new ModeControls(InputMode.Menu);
    public static ModeControls Other       = new ModeControls(InputMode.Other);

    private static ModeControls ActiveControls;

    protected static void OnPrimary()
    {
        ActiveControls.InvokePrimary();
    }

    protected static void OnSecondary()
    {
        ActiveControls.InvokeSecondary();
    }
}