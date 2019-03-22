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

    public void Start()
    {
        MenuManager.Instance.SetActiveMenu(this);
        itemSelectIndicator.gameObject.SetActive(false);
        UpdateSoundIcon();
    }

    void OnEnable()
    {
        PlayerManager.NewPlayerAdded += PlayerManager_NewPlayerAdded;
        PlayerManager.PlayerWasRemoved += PlayerManager_PlayerWasRemoved;
    }

    void OnDisable()
    {
        PlayerManager.NewPlayerAdded -= PlayerManager_NewPlayerAdded;
        PlayerManager.PlayerWasRemoved -= PlayerManager_PlayerWasRemoved;
    }

    private void PlayerManager_NewPlayerAdded(int total)
    {
        if (total != 1) return;
        MenuManager.Instance.StartIndicating();
    }

    private void PlayerManager_PlayerWasRemoved(int total)
    {
        if (total != 0) return;
        MenuManager.Instance.StartIndicating(false);
    }

    public void StartLevelSelect()
    {
        GameManager.Instance.StartLevelSelect();
    }

    public void ToggleSound()
    {
        if (soundToggleIcon == null) return;

        SoundManager.Instance.ToggleMute();
        UpdateSoundIcon();
    }

    public void UpdateSoundIcon()
    {
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
