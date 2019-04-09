using AccessibilityInputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour
{
    GameObject player;

    public void ReturnToMainMenu()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.SetParent(BasePlayerManager.Instance.playerParent);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Restart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.SetParent(BasePlayerManager.Instance.playerParent);
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
