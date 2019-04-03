using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    private void Start()
    {
        ColorChanger.SetRandomBackgroundColor();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void Resume()
    {
        GameManager.Instance.Resume();
    }

    public void Pause()
    {
        GameManager.Instance.Pause();
    }
}
