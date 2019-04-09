using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            NewSceneManager.Instance.StartGame();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            NewSceneManager.Instance.ExitGameFromMenu();
        }
    }
}
