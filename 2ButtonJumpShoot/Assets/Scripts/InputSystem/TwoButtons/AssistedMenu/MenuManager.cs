using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class MenuManager : MonoBehaviour
        {
            public static MenuManager Instance { get; private set; }

            public float autoInterval;

            [SerializeField, ReadOnly] private BaseMenuController activeMenuController;
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
                    DestroyImmediate(gameObject);
                }
            }

            private void OnEnable() => SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            private void OnDisable() => SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            private void SceneManager_activeSceneChanged(Scene from, Scene to) => Cleanup();
            public void SetActiveMenu(BaseMenuController menuController) => activeMenuController = menuController;
            void Cleanup() => StopAllCoroutines();

            void OnDestroy()
            {
                if (Instance == this) { Instance = null; }
                Cleanup();
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
                        case BaseMenuController.IndicatorMode.Single:
                            if (activeMenuController.itemSelectIndicator != null) activeMenuController.itemSelectIndicator.gameObject.SetActive(true);
                            break;
                        case BaseMenuController.IndicatorMode.RowAndSingle:
                            if (activeMenuController.rowSelectIndicator != null) activeMenuController.rowSelectIndicator.gameObject.SetActive(true);
                            break;
                        case BaseMenuController.IndicatorMode.ColumnAndSingle:
                            break;
                        case BaseMenuController.IndicatorMode.RowAndColumn:
                            break;
                        default:
                            break;
                    }
                    menuSelector = StartCoroutine(MenuSelection());
                }
                else if (menuSelector != null)
                {
                    if (activeMenuController.itemSelectIndicator != null) activeMenuController.itemSelectIndicator?.gameObject.SetActive(false);
                    StopCoroutine(menuSelector);
                    menuSelector = null;
                    HighlightButton(buttons[selectedButtonIndex], true);
                }
            }

            public void SelectButton()
            {
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
                Button selectedButton;
                buttons = new List<Button>(activeMenuController.buttonParent.GetComponentsInChildren<Button>());
                selectedButtonIndex = Mathf.Clamp(activeMenuController.startingIndex, 0, buttons.Count - 1);

                while (true)
                {
                    var buttonsPerColumn = activeMenuController.buttonsPerColumn;
                    var buttonsPerRow = activeMenuController.buttonsPerRow;

                    singleSelection = singleSelection || activeMenuController.indicatorMode == BaseMenuController.IndicatorMode.Single;
                    selectedButton = buttons[selectedButtonIndex];

                    // Indicate and Highlight
                    IndicateButton(selectedButton);

                    if (singleSelection)
                    {
                        HighlightButton(selectedButton);
                    }

                    yield return new WaitForSecondsRealtime(autoInterval);

                    if (singleSelection)
                    {
                        // Re-apply default sprite
                        HighlightButton(selectedButton, true);
                        selectedButtonIndex++;

                        switch (activeMenuController.indicatorMode)
                        {
                            case BaseMenuController.IndicatorMode.Single:
                                selectedButtonIndex %= buttons.Count;
                                break;
                            case BaseMenuController.IndicatorMode.RowAndSingle:
                                selectedButtonIndex = (selectedButtonIndex % buttonsPerRow) + currentRow * buttonsPerRow;
                                break;
                            case BaseMenuController.IndicatorMode.ColumnAndSingle:
                                break;
                            case BaseMenuController.IndicatorMode.RowAndColumn:
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        switch (activeMenuController.indicatorMode)
                        {
                            case BaseMenuController.IndicatorMode.RowAndSingle: // Row increment
                                selectedButtonIndex = (selectedButtonIndex + buttonsPerRow) % buttons.Count;
                                currentRow = Mathf.FloorToInt((float)selectedButtonIndex / buttonsPerRow);
                                break;
                            case BaseMenuController.IndicatorMode.ColumnAndSingle:
                                currentColumn = Mathf.FloorToInt((float)selectedButtonIndex / buttonsPerColumn);
                                break;
                            case BaseMenuController.IndicatorMode.RowAndColumn:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            void IndicateButton(Button btn)
            {
                try
                {

                    var btnRect = btn.GetComponent<RectTransform>();
                    var posX = new Vector2(btnRect.localPosition.x, 0);
                    var posY = new Vector2(0, btnRect.localPosition.y);

                    if (singleSelection)
                    {
                        if (activeMenuController.itemSelectIndicator != null)
                            activeMenuController.itemSelectIndicator.anchoredPosition = posX + posY + activeMenuController.itemIndicatorOffset;
                    }
                    else
                    {
                        switch (activeMenuController.indicatorMode)
                        {
                            case BaseMenuController.IndicatorMode.Single:
                                singleSelection = true;
                                IndicateButton(btn);
                                break;
                            case BaseMenuController.IndicatorMode.RowAndSingle:
                                if (activeMenuController.rowSelectIndicator != null)
                                    activeMenuController.rowSelectIndicator.anchoredPosition = posY + activeMenuController.rowIndicatorOffset;
                                break;
                            case BaseMenuController.IndicatorMode.ColumnAndSingle:
                                break;
                            case BaseMenuController.IndicatorMode.RowAndColumn:
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (System.Exception)
                {
                    return;
                }
            }

            void HighlightButton(Button btn, bool revert = false)
            {
                try
                {
                    if (btn.transition == Selectable.Transition.SpriteSwap)
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
                    else if (btn.transition == Selectable.Transition.Animation)
                    {
                        if (btn?.animator != null)
                        {
                            if (revert && btn?.animationTriggers?.normalTrigger != null)
                            {
                                btn.animator.SetBool(btn.animationTriggers.normalTrigger, true);
                                return;
                            }
                            btn.animator.SetBool(btn.animationTriggers.highlightedTrigger, true);
                        }
                    }
                }
                catch (System.Exception)
                {

                    return;
                }
            }
        }
    }
}