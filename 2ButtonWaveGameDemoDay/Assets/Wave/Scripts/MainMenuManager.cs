using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NewSceneManager.Instance.StartGame();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NewSceneManager.Instance.ExitGameFromMenu();
        }
    }
}
