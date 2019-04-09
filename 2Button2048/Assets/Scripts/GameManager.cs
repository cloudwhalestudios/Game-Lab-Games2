using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameController controller;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    protected void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void Restart()
    {
        controller.Restart();
    }

    internal void Resume()
    {
        controller.Pause(false);
    }

    internal void Pause()
    {
        controller.Pause();
    }
}
