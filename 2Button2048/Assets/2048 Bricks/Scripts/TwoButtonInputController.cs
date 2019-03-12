﻿using System;
using UnityEngine;

public abstract class TwoButtonInputController : MonoBehaviour
{
    public enum InputMode
    {
        Game,
        Pause,
        MainMenu
    }

    public struct ModeControls
    {
        public InputMode TargetMode;
        public event Action Primary;
        public event Action Secondary;

        public ModeControls(InputMode targetMode) : this()
        {
            TargetMode = targetMode;
        }

        public void InvokePrimary()
        {
            Primary.Invoke();
        }
        public void InvokeSecondary()
        {
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
                case InputMode.Pause:
                    ActiveControls = Pause;
                    break;
                case InputMode.MainMenu:
                    ActiveControls = MainMenu;
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
    public static ModeControls Pause       = new ModeControls(InputMode.Pause);
    public static ModeControls MainMenu    = new ModeControls(InputMode.MainMenu);

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