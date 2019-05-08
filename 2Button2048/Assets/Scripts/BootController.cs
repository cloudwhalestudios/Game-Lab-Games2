using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    [Header("Boot Options")]
    public float minBootDelay = 2f;
    public bool forceFullScreen = true;
    //public bool resetPlayerPrefs = false;

    //bool userIsSetup = false;
    bool finishedLoading = false;
    //bool interrupt = false;

    private void Awake()
    {
        // TODO Add loading behaviour
        Screen.fullScreen = forceFullScreen;
    }

    private void Start()
    {
        //AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Launch);
        StartCoroutine(StartNextScene());

        // Load the platform preferences
        LoadUserSetup();
    }

    private void LoadUserSetup()
    {
        BootLoader.LoadPlatformPlayer();
        finishedLoading = true;
    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSecondsRealtime(minBootDelay);

        if(!finishedLoading)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("MainMenuScene");
        yield break;
    }

}
