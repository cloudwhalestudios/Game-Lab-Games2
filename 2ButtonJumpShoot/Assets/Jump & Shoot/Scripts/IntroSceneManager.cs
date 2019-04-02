using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour {
    void Start()
    {
        StartCoroutine(GoToMainScene());
    }

    IEnumerator GoToMainScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenuScene");
        yield break;
    }
}
