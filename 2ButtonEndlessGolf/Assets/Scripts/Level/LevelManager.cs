using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Scene Management")]
    public string mainMenuSceneName;
    public string levelSelectSceneName;
    public List<LevelController> levelControllerPrefabs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadMainMenu()
    {
        OpenScene(mainMenuSceneName);
    }

    public void LoadLevelSelect()
    {
        OpenScene(levelSelectSceneName);
    }

    public void LoadLevel(LevelController controller)
    {
        // LevelSelect Scene    
        OpenScene(controller.sceneName);
    }

    public void ReloadScene()
    {
        OpenScene(SceneManager.GetActiveScene().name, true);
    }

    private void OpenScene(string name, bool reload = false)
    {
        if (reload || SceneManager.GetActiveScene().name != name)
            SceneManager.LoadScene(name);
    }
}
