using SgLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MenuController
{
    [Header("Toggle Config")]
    public Image soundToggleIcon;
    public Sprite SoundOn;
    public Sprite SoundOff;

    bool started = false;

    public void Start()
    {
        MenuManager.Instance.SetActiveMenu(this);
        menuSelectIndicator.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        PlayerManager.NewPlayerAdded += PlayerManager_NewPlayerAdded;
    }

    void OnDisable()
    {
        PlayerManager.NewPlayerAdded -= PlayerManager_NewPlayerAdded;
    }

    private void PlayerManager_NewPlayerAdded()
    {
        if (started) return;
        started = true;
        MenuManager.Instance.StartMoving();
    }

    public void StartLevelSelect()
    {
        GameManager.Instance.StartLevelSelect();
    }

    public void ToggleSound()
    {
        if (soundToggleIcon == null) return;

        SoundManager.Instance.ToggleMute();

        if (SoundManager.Instance.IsMuted())
        {
            soundToggleIcon.sprite = SoundOff;
        }
        else
        {
            soundToggleIcon.sprite = SoundOn;
        }
    }

    public void ExitGame()
    {
        GameManager.Instance.Quit();
    }
}
