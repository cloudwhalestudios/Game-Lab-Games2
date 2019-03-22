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

    [SerializeField] private Transform container;
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField, ReadOnly] private int angleStepCount;
    [SerializeField, ReadOnly] private int currentAngleStep;
    [SerializeField, ReadOnly] private int powerStepCount;
    [SerializeField, ReadOnly] private int currentPowerStep;
    [SerializeField, ReadOnly] private List<Image> powerStepImages;
    [SerializeField, ReadOnly] private Color currentColor;


    [SerializeField, ReadOnly] private float anglePercentageStep;
    [SerializeField, ReadOnly] private float powerPercentageStep;
    [SerializeField, ReadOnly] private float _currentAnglePercentage;
    [SerializeField, ReadOnly] private float _currentPowerPercentage;

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
        ResetValues();
        StartCoroutine(BounceAngle());
        StartCoroutine(BouncePower());

    }

    void SetupIndicators(Color color)
    {
        if (container == null || indicatorPrefab == null)
        {
            Debug.LogError("Hit indicator or prefab are not set!");
            return;
        }

        angleStepCount = GameManager.Instance.angleStepCount;
        powerStepCount = GameManager.Instance.powerStepCount;

        var minScale = new Vector3(GameManager.Instance.minPowerIndicatorScale, GameManager.Instance.minPowerIndicatorScale, 1f);
        var maxScale = new Vector3(GameManager.Instance.maxPowerIndicatorScale, GameManager.Instance.maxPowerIndicatorScale, 1f);

        currentColor = color;

        powerStepImages = new List<Image>();

        for (int i = 0; i < powerStepCount; i++)
        {
            var step = Instantiate(indicatorPrefab, container);
            step.transform.localScale = Vector3.Lerp(minScale, maxScale, (float) i / (powerStepCount - 1));
            var image = step.GetComponent<Image>();
            image.color = currentColor;
            powerStepImages.Add(image);
        }

        anglePercentageStep = 1f / (angleStepCount - 1);
        powerPercentageStep = 1f / (powerStepCount - 1);
    }

    public void SetPosition(Vector2 position)
    {
        container.position = position;
    }

    public void ResetValues()
    {
        SetupIndicators(PlayerManager.Instance.GetCurrentPlayerColor);

        currentPowerStep = 0;
        currentAngleStep = 0;
        CurrentAnglePercentage = 0f;
        CurrentPowerPercentage = 0f;
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
        //print("Bouncing power!");
        var invertFill = false;

        while(true)
        {
            yield return new WaitForSeconds(GameManager.Instance.autoInterval);

            var change = (invertFill) ? (-1) : 1;

            currentPowerStep += change;
            CurrentPowerPercentage = powerPercentageStep * currentPowerStep;

            invertFill = CheckFillDirection(invertFill, currentPowerStep, powerStepCount);
        }
    }

    IEnumerator BounceAngle()
    {
        //print("Bouncing angle!");
        var invertFill = false;
        while (true)
        {
            yield return new WaitForSeconds(GameManager.Instance.autoInterval);

            var change = (invertFill) ? -1 : 1;

            currentAngleStep += change;
            CurrentAnglePercentage = anglePercentageStep * currentAngleStep;

            invertFill = CheckFillDirection(invertFill, currentAngleStep, angleStepCount);
        }
    }

    bool CheckFillDirection (bool invertFill, int current, int max)
    {
        if (!invertFill && current >= max - 1)
        {
            return true;
        }
        else if (invertFill && current <= 0)
        {
            return false;
        }
        return invertFill;
    }

    void Update()
    {        
        // Update Graphics -> Indicators
        UpdateIndicatorAngle();
        UpdatePowerIndicator();
    }

    void UpdatePowerIndicator()
    {
        if (powerStepImages == null )
            return;

        for (int i = 0; i < powerStepImages.Count; i++)
        {
            if (i <= currentPowerStep)
            {
                powerStepImages[i].color = currentColor;
            }
            else
            { 
                powerStepImages[i].color = new Color(0,0,0,0);
            }
        }
    }

    void UpdateIndicatorAngle()
    {
        if (container == null)
            return;

        var rotation = Quaternion.identity;

        if (!PlayerManager.Instance.FlipAngle)
        {
            rotation = Quaternion.Lerp(
            Quaternion.Euler(0, 0, 180f - GameManager.Instance.minAngle),
            Quaternion.Euler(0f, 0f, 180f - GameManager.Instance.maxAngle),
            CurrentAnglePercentage);
        }
        else
        {
            rotation = Quaternion.Lerp(
            Quaternion.Euler(0, 0, GameManager.Instance.minAngle),
            Quaternion.Euler(0f, 0f, GameManager.Instance.maxAngle),
            CurrentAnglePercentage);
        }


        container.rotation = rotation;
    }
}
