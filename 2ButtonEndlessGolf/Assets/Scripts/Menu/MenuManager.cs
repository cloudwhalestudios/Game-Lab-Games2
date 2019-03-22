using SgLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField, ReadOnly] private MenuController activeMenuController;
    [SerializeField, ReadOnly] private int selectedButtonIndex;
    [SerializeField, ReadOnly] private List<Button> buttons;

    Coroutine menuSelector;

    Sprite lastDefaultStateSprite;
    Color lastDefaultColor;
    bool singleSelection;

    int currentColumn;
    int currentRow;

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
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
        Cleanup();
    }

    void Cleanup()
    {
        StopAllCoroutines();
    }

    public void SetActiveMenu(MenuController menuController)
    {
        activeMenuController = menuController;
        buttons = new List<Button>(menuController.GetComponentsInChildren<Button>());
    }

    public void ShowMenu(bool startMoving = true)
    {
        if (activeMenuController?.menuContainer != null)
        {
            activeMenuController.menuContainer.SetActive(true);
            StartIndicating(startMoving);
        }
    }
    public void HideMenu()
    {
        if (activeMenuController?.menuContainer != null)
        {
            activeMenuController.menuContainer.SetActive(false);
            StartIndicating(false);
        }
    }

    public void StartIndicating(bool indicate = true)
    {
        if (indicate)
        {
            switch (activeMenuController.indicatorMode)
            {
                case MenuController.IndicatorMode.Single:
                    activeMenuController.itemSelectIndicator.gameObject.SetActive(true);
                    break;
                case MenuController.IndicatorMode.RowAndSingle:
                    activeMenuController.rowSelectIndicator.gameObject.SetActive(true);
                    break;
                case MenuController.IndicatorMode.ColumnAndSingle:
                    break;
                case MenuController.IndicatorMode.RowAndColumn:
                    break;
                default:
                    break;
            }
            menuSelector = StartCoroutine(MenuSelection());
        }
        else if (menuSelector != null)
        {
            activeMenuController.itemSelectIndicator.gameObject.SetActive(false);
            StopCoroutine(menuSelector);
            menuSelector = null;
            HighlightButton(buttons[selectedButtonIndex], true);
        }
    }

    public void SelectButton()
    {
        if (SoundManager.Instance?.button != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        }

        if (!singleSelection)
        {
            singleSelection = true;
        }
        else
        {
            buttons[selectedButtonIndex].onClick.Invoke();
        }
    }

    public bool EnableMultiSelection()
    {
        if (!singleSelection) return false;

        selectedButtonIndex = (selectedButtonIndex % activeMenuController.buttonsPerRow) + currentRow * activeMenuController.buttonsPerRow;
        HighlightButton(buttons[selectedButtonIndex], true);
        singleSelection = false;
        return true;
    }

    IEnumerator MenuSelection()
    {
        selectedButtonIndex = activeMenuController.startingIndex;
        
        yield return null;
        Button selectedButton;

        
        while (true)
        {
            var buttonsPerColumn = activeMenuController.buttonsPerColumn;
            var buttonsPerRow = activeMenuController.buttonsPerRow;

            singleSelection = singleSelection || activeMenuController.indicatorMode == MenuController.IndicatorMode.Single;
            selectedButton = buttons[selectedButtonIndex];

            // Indicate and Highlight
            IndicateButton(selectedButton);

            if (singleSelection)
            {
                HighlightButton(selectedButton);
            }
            
            yield return new WaitForSecondsRealtime(GameManager.Instance.autoInterval);

            if (singleSelection)
            {
                // Re-apply default sprite
                HighlightButton(selectedButton, true);
                selectedButtonIndex++;

                switch (activeMenuController.indicatorMode)
                {
                    case MenuController.IndicatorMode.Single:
                        selectedButtonIndex %= buttons.Count;
                        break;
                    case MenuController.IndicatorMode.RowAndSingle:
                        selectedButtonIndex = (selectedButtonIndex % buttonsPerRow) + currentRow * buttonsPerRow;
                        break;
                    case MenuController.IndicatorMode.ColumnAndSingle:
                        break;
                    case MenuController.IndicatorMode.RowAndColumn:
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                switch (activeMenuController.indicatorMode)
                {
                    case MenuController.IndicatorMode.RowAndSingle: // Row increment
                        selectedButtonIndex = (selectedButtonIndex + buttonsPerRow) % buttons.Count;
                        currentRow = Mathf.FloorToInt((float)selectedButtonIndex / buttonsPerRow);
                        break;
                    case MenuController.IndicatorMode.ColumnAndSingle:
                        currentColumn = Mathf.FloorToInt((float)selectedButtonIndex / buttonsPerColumn);
                        break;
                    case MenuController.IndicatorMode.RowAndColumn:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void IndicateButton(Button btn)
    {
        var btnRect = btn.GetComponent<RectTransform>();
        var posX = new Vector2(btnRect.localPosition.x, 0);
        var posY = new Vector2(0, btnRect.localPosition.y);

        if (singleSelection) { 
            activeMenuController.itemSelectIndicator.anchoredPosition = posX + posY + activeMenuController.itemIndicatorOffset;
        }
        else
        {
            switch (activeMenuController.indicatorMode)
            {
                case MenuController.IndicatorMode.Single:
                    singleSelection = true;
                    IndicateButton(btn);
                    break;
                case MenuController.IndicatorMode.RowAndSingle:
                    activeMenuController.rowSelectIndicator.anchoredPosition = posY + activeMenuController.rowIndicatorOffset;
                    break;
                case MenuController.IndicatorMode.ColumnAndSingle:
                    break;
                case MenuController.IndicatorMode.RowAndColumn:
                    break;
                default:
                    break;
            }
        }
    }

    void HighlightButton(Button btn, bool revert = false)
    {
        if (revert && btn?.image?.sprite != null)
        {
            if (lastDefaultStateSprite != null) btn.image.sprite = lastDefaultStateSprite;
            if (lastDefaultColor != null) btn.image.color = lastDefaultColor;
            return;
        }

        lastDefaultStateSprite = btn.image.sprite;
        lastDefaultColor = btn.image.color;
        btn.image.sprite = btn.spriteState.highlightedSprite;
        btn.image.color = Color.white;
    }
}