using System;
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

            public int direction = 1;

            [SerializeField, ReadOnly] private BaseMenuController controller;
            [SerializeField, ReadOnly] private int selectedIndex;
            [SerializeField, ReadOnly] private List<Button> buttons;

            Coroutine menuSelector;
            Coroutine timerUpdateRoutine;

            Sprite lastDefaultStateSprite;
            Color lastDefaultColor;

            bool firstTimeAnimation = true;

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

            public void SetMenuController(BaseMenuController menuController)
            {
                if (controller != null && controller.updateStartIndex) controller.startingIndex = selectedIndex;

                controller = menuController;
                buttons = new List<Button>(controller.buttonParent.GetComponentsInChildren<Button>());
                selectedIndex = Mathf.Clamp(controller.startingIndex, 0, buttons.Count - 1);
                firstTimeAnimation = true;
            }

            void Cleanup()
            {
                if (menuSelector != null) StopCoroutine(menuSelector);
                menuSelector = null;
                if (timerUpdateRoutine != null) StopCoroutine(timerUpdateRoutine);
                timerUpdateRoutine = null;
            }

            void OnDestroy()
            {
                if (Instance == this) { Instance = null; }
                Cleanup();
            }

            public void ShowMenu(bool startMoving = true)
            {
                if (controller?.menuContainer != null)
                {
                    controller.menuContainer.SetActive(true);
                    StartIndicating(startMoving);
                }
            }
            public void HideMenu()
            {
                if (controller?.menuContainer != null)
                {
                    controller.menuContainer.SetActive(false);
                    StartIndicating(false);
                }
            }

            public void StartIndicating(bool indicate = true)
            {
                if (indicate)
                {
                    //Debug.Log(controller?.name + " " + controller?.buttonParent);

                    if (controller.itemSelectIndicator != null) controller.itemSelectIndicator.gameObject.SetActive(true);
                    if (controller.itemSelectTimer != null) controller.itemSelectTimer.gameObject.SetActive(true);

                    if (controller.transitionType == BaseMenuController.Transition.Animate) SetupButtonAnimation();
                    else if (controller.transitionType == BaseMenuController.Transition.Move) SetupButtonIndication();
                    
                    menuSelector = StartCoroutine(MenuSelection());
                }
                else
                {
                    if (controller.itemSelectIndicator != null) controller.itemSelectIndicator?.gameObject.SetActive(false);
                    if (controller.itemSelectTimer != null) controller.itemSelectTimer.gameObject.SetActive(false);
                    Cleanup();
                    HighlightButton(buttons[selectedIndex], true);
                }
            }

            private void SetupButtonAnimation()
            {
                UpdateAnimationSpots();
                controller.firstSpot.buttonObject.transform.localPosition = controller.firstSpot.localPosition;
                controller.currentSpot.buttonObject.transform.localPosition = controller.currentSpot.localPosition;
                controller.lastSpot.buttonObject.transform.localPosition = controller.lastSpot.localPosition;

                controller.firstSpot.buttonObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                controller.currentSpot.buttonObject.transform.localScale = new Vector3(1f, 1f, 1f);
                controller.lastSpot.buttonObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }

            private void SetupButtonIndication()
            {
                IndicateButton(buttons[selectedIndex]);
            }

            private void UpdateAnimationSpots()
            {
                var firstIndex = (selectedIndex + buttons.Count - direction) % buttons.Count;
                var lastIndex = (selectedIndex + buttons.Count + direction) % buttons.Count;

                var first = buttons[firstIndex].gameObject;
                var current = buttons[selectedIndex].gameObject;
                var last = buttons[lastIndex].gameObject;

                controller.firstSpot.buttonObject = first;
                controller.currentSpot.buttonObject = current;
                controller.lastSpot.buttonObject = last;

                var canvas = current.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 10;
                }
                canvas = first.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.overrideSorting = false;
                    canvas.sortingOrder = 0;
                }
                canvas = last.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.overrideSorting = false;
                    canvas.sortingOrder = 0;
                }
            }

            public void SelectItem()
            {
                //Debug.Log($"Selecting button {buttons[selectedIndex].name} ({selectedIndex})");
                buttons[selectedIndex].onClick.Invoke();
            }

            IEnumerator MenuSelection()
            {
                Button selectedButton;
                yield return null;
                HighlightButton(buttons[selectedIndex]);

                while (true)
                {
                    selectedButton = buttons[selectedIndex];

                    // Indicate and Highlight
                    //AudioManager.Instance?.PlaySoundNormally(AudioManager.Instance?.Select);
                    switch (controller.transitionType)
                    {
                        case BaseMenuController.Transition.Move:
                            IndicateButton(selectedButton);
                            break;
                        case BaseMenuController.Transition.Animate:
                            AnimateButton(selectedButton);
                            break;
                        default:
                            break;
                    }

                    HighlightButton(selectedButton);
                    
                    if (controller.itemSelectTimer != null)
                    {

                        yield return timerUpdateRoutine = StartCoroutine(UpdateTimerProgress(PlatformPreferences.Current.MenuProgressionTimer));
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(PlatformPreferences.Current.MenuProgressionTimer);
                    }

                    HighlightButton(selectedButton, true);
                    selectedIndex = (selectedIndex + buttons.Count + direction) % buttons.Count;
                }
            }

            private void AnimateButton(Button selectedButton)
            {
                try
                {
                    StartCoroutine(AnimateButtonCoroutine(selectedButton));
                }
                catch (Exception)
                {

                    return;
                }
            }

            IEnumerator AnimateButtonCoroutine(Button selectedButton)
            {
                // Fade first
                StartCoroutine(AnimateMoveItem(
                        controller.firstSpot.buttonObject,
                        controller.firstSpot.localPosition,
                        controller.firstSpot.localPosition,
                        controller.disappearClip,
                        controller.staticFadedClip,
                        controller.transitionTime
                        ));
                UpdateAnimationSpots();

                // Appear last
                StartCoroutine(AnimateMoveItem(
                            controller.lastSpot.buttonObject,
                            controller.lastSpot.localPosition,
                            controller.lastSpot.localPosition,
                            controller.appearClip,
                            controller.staticSmallClip,
                            controller.transitionTime
                        ));
                
                // Move to current
                StartCoroutine(AnimateMoveItem(
                            controller.currentSpot.buttonObject,
                            controller.lastSpot.localPosition,
                            controller.currentSpot.localPosition,
                            controller.growClip,
                            controller.staticLargeClip,
                            controller.transitionTime
                        ));

                // Move to first
                StartCoroutine(AnimateMoveItem(
                            controller.firstSpot.buttonObject,
                            controller.currentSpot.localPosition,
                            controller.firstSpot.localPosition,
                            controller.shrinkClip,
                            controller.staticSmallClip,
                            controller.transitionTime
                        ));

                if (firstTimeAnimation)
                {
                    firstTimeAnimation = false;
                }

                yield return null;
            }

            IEnumerator AnimateMoveItem(GameObject item, Vector3 startLocation, Vector3 endLocation, AnimationClip transitionClip, AnimationClip staticClip, float transitionTime)
            {
                var animation = item.GetComponent<Animation>();
                if (animation == null)
                {
                    animation = item.AddComponent<Animation>();
                }
                if (animation.GetClip(transitionClip.name) == null)
                {
                    transitionClip.legacy = true;
                    transitionClip.wrapMode = WrapMode.Once;
                    animation.AddClip(transitionClip, transitionClip.name);
                }
                if (animation.GetClip(staticClip.name) == null)
                {
                    staticClip.legacy = true;
                    staticClip.wrapMode = WrapMode.Once;
                    animation.AddClip(staticClip, staticClip.name);
                }
                animation.Stop();

                if (!firstTimeAnimation)
                {
                    animation.Play(transitionClip.name);
                    animation[transitionClip.name].speed = transitionClip.length / transitionTime;

                    item.transform.localPosition = startLocation;
                    if (startLocation != endLocation)
                    {
                        var elapsedTime = 0f;
                        while (elapsedTime <= transitionTime)
                        {
                            yield return null;
                            elapsedTime += Time.unscaledDeltaTime;
                            var percentage = Mathf.Clamp01(elapsedTime / transitionTime);
                            item.transform.localPosition = Vector3.Lerp(startLocation, endLocation, percentage);
                        }
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(transitionTime);
                    }
                }
                else
                {
                    item.transform.localPosition = endLocation;
                }
                
                animation.Stop();
                animation.Play(staticClip.name);
            }

            IEnumerator UpdateTimerProgress(float waitTime)
            {
                var elapsedTime = 0f;
                controller.itemSelectTimer.localScale = new Vector3(0, 1, 1);
                while (elapsedTime < waitTime)
                {
                    yield return null;
                    elapsedTime += Time.unscaledDeltaTime;
                    var percentage = Mathf.Clamp01(elapsedTime / waitTime);
                    controller.itemSelectTimer.localScale = new Vector3(percentage, 1, 1);
                }
            }

            void IndicateButton(Button btn)
            {
                try
                {
                    var btnRect = btn.GetComponent<RectTransform>();
                    var posX = new Vector2(btnRect.localPosition.x, 0);
                    var posY = new Vector2(0, btnRect.localPosition.y);

                    if (controller.itemSelectIndicator != null)
                        controller.itemSelectIndicator.anchoredPosition = posX + posY + controller.itemIndicatorOffset;
                   
                }
                catch (Exception)
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
                catch (Exception)
                {

                    return;
                }
            }
        }
    }
}