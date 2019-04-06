using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) { 
            //player.transform.SetParent(BasePlayerManager.Instance.playerParent);
            SceneManager.LoadScene("MainScene");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("../");
#else
        Application.Quit();
#endif
    }
}
