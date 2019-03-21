using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfHitManager : MonoBehaviour
{
    public static GolfHitManager Instance { get; private set; }

    public float CurrentPowerPercentage
    {
        get { return _currentPowerPercentage; }
        private set
        {
            
            _currentPowerPercentage = Mathf.Clamp01(value);
        }
    }

    public float CurrentAnglePercentage
    {
        get { return _currentAnglePercentage; }
        private set
        {
            _currentAnglePercentage = Mathf.Clamp01(value);
        }
    }

    [SerializeField] private RectTransform angleIndicator;
    [SerializeField] private RectTransform powerIndicator;
    [Range(0.01f, 1f)]
    [SerializeField] private float angleStep = .33f;
    [Range(0.01f, 1f)]
    [SerializeField] private float powerStep = .33f;
    private float stepInterval;

    private float _currentAnglePercentage;
    private float _currentPowerPercentage;
    private bool invertFill;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void Start()
    {
        SetupIndicators();
        ResetValues();
    }

    void SetupIndicators()
    {
        if (angleIndicator == null || powerIndicator == null)
        {
            Debug.LogError("Angle Indicator or Power Indicator are not set!");
            return;
        }

        angleIndicator.pivot = new Vector2(0, 0);
        powerIndicator.pivot = new Vector2(0.5f, 0);
    }

    void ResetValues()
    {
        CurrentAnglePercentage = 0f;
        CurrentPowerPercentage = 0f;
        invertFill = false;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnEnable()
    {
        GameManager.TurnStateChanged += OnTurnStateChanged;
    }

    void OnDisable()
    {
        GameManager.TurnStateChanged -= OnTurnStateChanged;
    }

    void OnTurnStateChanged(TurnState newState, TurnState oldState)
    {
        switch (newState)
        {
            case TurnState.Start:
                // Pause/reset all values/bars
                ResetValues();
                break;
            case TurnState.Angle:
                // Stop bounces
                StopBouncing();
                // Start angle bounce
                StartCoroutine(BounceAngle());
                break;
            case TurnState.Power:
                // Stop bounces
                StopBouncing();
                // Start power bounce
                StartCoroutine(BouncePower());
                break;
            case TurnState.Firing:
                // Stop bounces
                StopBouncing();
                break;
            default:
                break;
        }
    }

    void StopBouncing()
    {
        StopAllCoroutines();
    }

    IEnumerator BouncePower()
    {
        print("Bouncing power!");
        while(true)
        {
            yield return new WaitForSeconds(stepInterval);
            CurrentPowerPercentage += (invertFill) ? -powerStep : powerStep;
            CheckFillDirection(CurrentPowerPercentage);
        }
    }

    IEnumerator BounceAngle()
    {
        print("Bouncing angle!");
        while (true)
        {
            yield return new WaitForSeconds(stepInterval);
            CurrentAnglePercentage += (invertFill) ? -angleStep : angleStep;
            CheckFillDirection(CurrentAnglePercentage);
        }
    }

    void CheckFillDirection (float percentage)
    {
        if (!invertFill && percentage >= 1)
        {
            invertFill = true;
        }
        else if (invertFill && percentage <= 0)
        {
            invertFill = false;
        }
    }

    void Update()
    {
        stepInterval = GameManager.Instance.autoInterval;
        
        // Update Graphics -> Indicators
        UpdateAngleUI();
        UpdatePowerUI();
    }

    void UpdatePowerUI()
    {
        if (powerIndicator == null)
            return;

        var scale = powerIndicator.localScale;
        scale.y = CurrentPowerPercentage;
        powerIndicator.localScale = scale;
    }

    void UpdateAngleUI()
    {
        if (angleIndicator == null)
            return;

        var rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 90f), CurrentAnglePercentage);
        angleIndicator.rotation = rotation;
    }
}
