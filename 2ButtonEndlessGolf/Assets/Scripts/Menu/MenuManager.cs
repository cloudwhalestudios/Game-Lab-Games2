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
        if (Instance == this) { Instance = null; }
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
            activeMenuController.menuSelectIndicator.gameObject.SetActive(true);
            menuSelector = StartCoroutine(MenuSelection());
        }
        else if (menuSelector != null)
        {
            activeMenuController.menuSelectIndicator.gameObject.SetActive(false);
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
        buttons[selectedButtonIndex].onClick.Invoke();
    }

    IEnumerator MenuSelection()
    {
        selectedButtonIndex = activeMenuController.startingIndex;
        yield return null;
        Button selectedButton;
        
        while (true)
        {
            selectedButton = buttons[selectedButtonIndex];

            // Indicate and Highlight
            IndicateButton(selectedButton);
            HighlightButton(selectedButton);

            yield return new WaitForSecondsRealtime(GameManager.Instance.autoInterval);

            // Re-apply default sprite
            HighlightButton(selectedButton, true);

            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
        }
    }

    void IndicateButton(Button btn)
    {
        var btnRect = btn.GetComponent<RectTransform>();
        var pos = new Vector2(btnRect.localPosition.x, btnRect.localPosition.y);

        activeMenuController.menuSelectIndicator.anchoredPosition = pos + activeMenuController.offset;
    }

    void HighlightButton(Button btn, bool revert = false)
    {
        if (revert)
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