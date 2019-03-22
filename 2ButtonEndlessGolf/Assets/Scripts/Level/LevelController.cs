using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public abstract class LevelController : MonoBehaviour
{
    public string sceneName;

    protected virtual void Start()
    {
        if (sceneName == "")
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
    }
}
