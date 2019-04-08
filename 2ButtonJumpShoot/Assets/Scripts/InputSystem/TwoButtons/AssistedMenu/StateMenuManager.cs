using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class StateMenuManager : MonoBehaviour
        {
            public static StateMenuManager Instance { get; private set; }

            public bool hideHighlightOnSelect = true;
            public int direction = 1;

            [SerializeField, ReadOnly] private BaseStateMenuController controller;
            [SerializeField, ReadOnly] private int selectedStateIndex;

            Coroutine stateSelector;
            Coroutine timerUpdateRoutine;

            StateMenu lastHighlightedState;

            BaseStateMenuController.Mode currentMode;

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

            public void SetStateMenuController(BaseStateMenuController controller)
            {
                this.controller = controller;
                currentMode = controller.indicatorMode;
                controller.stateSelectTimer?.gameObject.SetActive(false);

                selectedStateIndex = Mathf.Clamp(controller.startStateIndex, 0, controller.stateMenus.Count - 1);

                var stateMenu = controller.stateMenus[selectedStateIndex];
                if (currentMode == BaseStateMenuController.Mode.Single)
                {
                    stateMenu.selectEvent?.Invoke();
                    stateMenu.highlight.SetActive(hideHighlightOnSelect);
                }
                else if (currentMode == BaseStateMenuController.Mode.State)
                {
                    controller.stateSelectTimer?.gameObject.SetActive(true);
                    if (controller.stateSelectTimer != null)
                    {
                        controller.stateSelectTimer.localScale = new Vector3(0, 1f, 1f);
                    }
                }
            }

            private void Cleanup()
            {
                lastHighlightedState = null;

                if (stateSelector != null) StopCoroutine(stateSelector);
                stateSelector = null;

                if (timerUpdateRoutine != null) StopCoroutine(timerUpdateRoutine);
                timerUpdateRoutine = null;
            }

            void OnDestroy()
            {
                if (Instance == this) { Instance = null; }
                Cleanup();
            }

            public void Select()
            {
                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        // Select button from active menu
                        MenuManager.Instance.SelectItem();
                        break;

                    case BaseStateMenuController.Mode.State:
                        Debug.Log($"Selecting State {controller.stateMenus[selectedStateIndex].name} ({selectedStateIndex})");
                        //AudioManager.Instance?.PlaySoundNormally(AudioManager.Instance?.Accept);
                        // Stop state indication
                        StartIndicating(false);
                        if (hideHighlightOnSelect)
                        {
                            lastHighlightedState.highlight.SetActive(false);
                        }

                        // Start menu indication
                        currentMode = BaseStateMenuController.Mode.Single;

                        var selectedState = controller.stateMenus[selectedStateIndex];
                        selectedState.selectEvent?.Invoke();

                        StartIndicating();
                        break;
                }
            }

            public void Return()
            {
                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        // Stop menu indication
                        StartIndicating(false);

                        var selectedState = controller.stateMenus[selectedStateIndex];
                        selectedState.returnEvent?.Invoke();

                        // Start state indication
                        currentMode = BaseStateMenuController.Mode.State;

                        if (hideHighlightOnSelect)
                        {
                            lastHighlightedState?.highlight.SetActive(true);
                        }

                        StartIndicating();
                        break;
                    case BaseStateMenuController.Mode.State:
                        // TODO ??

                        break;
                }
            }

            public void StartIndicating(bool start = true)
            {
                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        var selectedState = controller.stateMenus[selectedStateIndex];
                        MenuManager.Instance.SetMenuController(selectedState.menuController);

                        MenuManager.Instance.StartIndicating(start);
                        break;
                    case BaseStateMenuController.Mode.State:
                        // Start state indication
                        if (start)
                        {
                            controller.stateSelectTimer?.gameObject.SetActive(true);
                            if (controller.stateSelectTimer != null)
                            {
                                controller.stateSelectTimer.localScale = new Vector3(0, 1f, 1f);
                            }

                            stateSelector = StartCoroutine(StateSelection());
                        }
                        else
                        {
                            Cleanup();
                            if (stateSelector != null) stateSelector = null;

                            controller.stateSelectTimer?.gameObject.SetActive(false);
                        }
                        break;
                }
            }

            IEnumerator StateSelection()
            {
                StateMenu selectedState;
                selectedStateIndex = Mathf.Clamp(selectedStateIndex, 0, controller.stateMenus.Count - 1);
                yield return null;
                HighlightState(controller.stateMenus[selectedStateIndex]);

                while (true)
                {
                    selectedState = controller.stateMenus[selectedStateIndex];
                    //AudioManager.Instance?.PlaySoundNormally(AudioManager.Instance?.Select);
                    HighlightState(selectedState);


                    if (controller.stateSelectTimer != null)
                    {
                        yield return timerUpdateRoutine = StartCoroutine(UpdateTimerProgress(controller.stateSelectTimer, PlatformPreferences.Current.MenuProgressionTimer));
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(PlatformPreferences.Current.MenuProgressionTimer);
                    }

                    selectedStateIndex = (selectedStateIndex + controller.stateMenus.Count + direction) % controller.stateMenus.Count;
                }
            }

            IEnumerator UpdateTimerProgress(RectTransform timer, float waitTime)
            {
                //Debug.Log("Timer ...");
                var elapsedTime = 0f;
                timer.localScale = new Vector3(0, 1, 1);
                while (elapsedTime < waitTime)
                {
                    yield return null;
                    elapsedTime += Time.unscaledDeltaTime;
                    var percentage = Mathf.Clamp01(elapsedTime / waitTime);
                    //Debug.Log($"Percent: {percentage * 100f}%; Elapsed: {elapsedTime}s; Wait: {waitTime}s");
                    timer.localScale = new Vector3(percentage, 1, 1);
                }
            }

            void HighlightState(StateMenu state)
            {
                try
                {
                    if (lastHighlightedState != null)
                    {
                        lastHighlightedState.highlight?.SetActive(false);
                    }
                    state.highlight.SetActive(true);
                    lastHighlightedState = state;
                }
                catch (System.Exception)
                {
                    Debug.Log("Exeption");
                    return;
                }
            }
            
        }
    }
}