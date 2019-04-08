using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour {
    void Start()
    {
        ColorChanger.SetRandomBackgroundColor();
        StartCoroutine(GoToMainScene());
    }

    IEnumerator GoToMainScene()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Splash);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenuScene");
        yield break;
    }
}
