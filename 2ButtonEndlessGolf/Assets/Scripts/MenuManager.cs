using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private MenuController activeMenuController;

    int selectedButtonIndex;
    List<Button> buttons;

    Coroutine menuSelector;

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
            StartMoving(startMoving);
        }
    }
    public void HideMenu()
    {
        if (activeMenuController?.menuContainer != null)
        {
            activeMenuController.menuContainer.SetActive(false);
            StartMoving(false);
        }
    }

    public void StartMoving(bool start = true)
    {
        if (start)
        {
            menuSelector = StartCoroutine(MenuSelection());
        }
        else if (menuSelector != null)
        {
            
            StopCoroutine(menuSelector);
            menuSelector = null;
        }
    }

    public void SelectButton()
    {
        buttons[selectedButtonIndex].onClick.Invoke();
    }

    IEnumerator MenuSelection()
    {
        selectedButtonIndex = activeMenuController.startingIndex;
        yield return null;
        Button selectedButton;
        Sprite defaultStateSprite;
        Color defaultColor;
        while (true)
        {
            selectedButton = buttons[selectedButtonIndex];
            defaultStateSprite = selectedButton.image.sprite;
            defaultColor = selectedButton.image.color;
            Debug.Log(selectedButton.image);

            IndicateMenuButton(selectedButton);
            
            // Apply Highlight
            selectedButton.image.sprite = selectedButton.spriteState.highlightedSprite;
            selectedButton.image.color = Color.white;

            yield return new WaitForSecondsRealtime(GameManager.Instance.autoInterval);

            // Re-apply default sprite
            selectedButton.image.sprite = defaultStateSprite;
            selectedButton.image.color = defaultColor;


            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
        }
    }

    void IndicateMenuButton(Button btn)
    {
        var btnRect = btn.GetComponent<RectTransform>();
        var pos = new Vector2(btnRect.localPosition.x, btnRect.localPosition.y);

        activeMenuController.menuSelectIndicator.anchoredPosition = pos + activeMenuController.offset;
    }
}