using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : ActiveInputHandler
{
    [SerializeField] private Player gamePlayerComponent;
    [SerializeField] private PlayerPlaceholder menuPlayerComponent;

    enum InputMode
    {
        Disabled, Menu, Game
    }
    InputMode currentInputMode = InputMode.Disabled;

    private void Awake()
    {
        if (gamePlayerComponent == null)
        {
            gamePlayerComponent = GetComponent<Player>();
        }
        if (menuPlayerComponent == null)
        {
            menuPlayerComponent = GetComponent<PlayerPlaceholder>();
        }
    }

    private void Start()
    {
        SceneManager_activeSceneChanged(default, SceneManager.GetActiveScene());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        switch (to.name)
        {
            case "MainScene":
                SetupGameplay();
                break;
            case "MainMenuScene":
                SetupMenu();
                break;
            default:
                break;
        }
        transform.parent = BasePlayerManager.Instance.playerParent;
    }

    private void SetupMenu()
    {
        gamePlayerComponent.enabled = false;
        menuPlayerComponent.enabled = true;
        menuPlayerComponent.InitPlaceholder();
    }

    private void SetupGameplay()
    {
        menuPlayerComponent.enabled = false;
        gamePlayerComponent.enabled = true;
        gamePlayerComponent.InitPlayer();
    }

    void UpdateInputMode()
    {
        //Debug.Log("Is paused/menu: " + GameManager.Instance != null && TimeScaleController.Instance != null && !TimeScaleController.Instance.IsPaused);
        if (GameManager.Instance != null && TimeScaleController.Instance != null && !TimeScaleController.Instance.IsPaused)
        {
            currentInputMode = InputMode.Game;
        }
        else
        {
            currentInputMode = InputMode.Menu;
        }
    }

    protected override void TBPrimary_InputEvent(KeyCode primaryKey)
    {
        UpdateInputMode();

        switch (currentInputMode)
        {
            case InputMode.Menu:
                // Select Button
                MenuManager.Instance.SelectItem();
                break;

            case InputMode.Game:
                if (gamePlayerComponent.currentState == Player.PlayerState.Standing)
                {
                    gamePlayerComponent.jump = true;
                }
                else if (gamePlayerComponent.currentState == Player.PlayerState.Jumping)
                {
                    gamePlayerComponent.shoot = true;
                }
                break;
            default:
                break;
        }
    }

    protected override void TBSecondary_InputEvent(KeyCode secondaryKey)
    {
        UpdateInputMode();
        switch (currentInputMode)
        {
            case InputMode.Game:
                // Pause Game
                GameManager.Instance.Pause();
                break;
            default:
                break;
        }
    }
}
