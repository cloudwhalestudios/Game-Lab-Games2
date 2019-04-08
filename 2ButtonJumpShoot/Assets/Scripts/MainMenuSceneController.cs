using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneController : MonoBehaviour
{
    private void Start()
    {
        ColorChanger.SetRandomBackgroundColor();
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) { 
            player.transform.SetParent(BasePlayerManager.Instance.playerParent);
            SceneManager.LoadScene("MainScene");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        WebGLIntegration.WebGLRedirect.OpenLauncher();
#else
        Application.Quit();
#endif
    }
}
