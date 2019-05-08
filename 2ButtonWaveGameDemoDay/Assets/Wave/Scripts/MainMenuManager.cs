using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] KeyCode primaryKey;
    [SerializeField] KeyCode secondaryKey;

    private void Start()
    {
        if (PlatformPreferences.Current?.Keys != null)
        {
            primaryKey = PlatformPreferences.Current.Keys[0];
            secondaryKey = PlatformPreferences.Current.Keys[1];
        }
        else if (primaryKey == KeyCode.None || secondaryKey == KeyCode.None)
        {
            primaryKey = KeyCode.LeftArrow;
            secondaryKey = KeyCode.RightArrow;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(primaryKey))
        {
            NewSceneManager.Instance.StartGame();
        }
        if (Input.GetKeyDown(secondaryKey))
        {
            NewSceneManager.Instance.ExitGameFromMenu();
        }
    }
}
