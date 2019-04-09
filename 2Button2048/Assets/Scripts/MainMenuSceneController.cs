using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneController : MonoBehaviour
{
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
