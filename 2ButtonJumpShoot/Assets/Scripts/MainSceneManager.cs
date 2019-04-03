using AccessibilityInputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ColorChanger.SetRandomBackgroundColor();
    }

    public void ReturnToMainMenu()
    {
        player.transform.SetParent(BasePlayerManager.Instance.playerParent);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Restart()
    {
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
