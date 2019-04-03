using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player gamePlayerComponent;
    [SerializeField] private PlayerPlaceholder menuPlayerComponent;

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

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }
    private void OnDisable()
    {
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
}
