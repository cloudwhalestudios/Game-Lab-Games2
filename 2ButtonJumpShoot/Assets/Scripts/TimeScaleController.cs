﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    enum ScaleMode
    {
        Standard,
        SlowMotion,
        Paused
    }

    public static TimeScaleController Instance { get; private set; }

    public float defaultTimeScale = 1f;
    public float slowMotionTimeScale = 0.5f;
    public float pauseTimeScale = 0f;

    float unpausedTimeScale = 0f;

    public bool IsPaused() => Time.timeScale == pauseTimeScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance);
            Instance = this;
        }

        ApplyTimeScale(defaultTimeScale);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void ApplyTimeScale(float scale, bool isPause = false)
    {
        if (isPause)
        {
            // remember the last active timescale
            unpausedTimeScale = Time.timeScale;
        }

        Time.timeScale = scale;
    }

    public void SetSlowMotionActive(bool activate = true)
    {
        if (activate)
        {
            ApplyTimeScale(slowMotionTimeScale);
        }
        else
        {
            ApplyTimeScale(defaultTimeScale);
        }
        Debug.Log(Time.timeScale);
    }

    public void Pause(bool pause = true)
    {
        if (pause)
        {
            ApplyTimeScale(pauseTimeScale, true);
        }
        else
        {
            ApplyTimeScale(unpausedTimeScale);
        }
    }
}