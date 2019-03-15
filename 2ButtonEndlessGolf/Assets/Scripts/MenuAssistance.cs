using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class MenuAssistance : MonoBehaviour
{
    public float autoMoveInterval;
    public GameObject pauseMenu;
    public GameObject pauseButtonParent;
    public RectTransform menuSelectIndicator;
    public int startingIndex;
    public bool startPaused;

    int selectedButtonIndex;
    List<Button> buttons;

    Coroutine menuSelector;

    void Start()
    {
        InputController.Pause.Primary += OnPrimaryPause;
        InputController.Pause.Secondary += OnSecondaryPause;
    }

    void OnDestroy()
    {
        InputController.Pause.Primary -= OnPrimaryPause;
        InputController.Pause.Secondary -= OnSecondaryPause;

        StopAllCoroutines();
    }

    void OnPrimaryPause()
    {
        // perform select button action
        buttons[selectedButtonIndex].onClick.Invoke();
    }

    void OnSecondaryPause()
    {
        Quit();
    }

    private void SetupPauseMenu()
    {
        pauseMenu.SetActive(false);

        buttons = new List<Button>(pauseButtonParent.GetComponentsInChildren<Button>());

        if (startPaused)
        {
            PauseGame();
        }
    }   

    IEnumerator MenuSelection()
    {
        selectedButtonIndex = startingIndex;
        yield return null;
        while (true)
        {
            Debug.Log("Selection " + selectedButtonIndex);
            IndicateMenuButton(selectedButtonIndex);
            yield return new WaitForSecondsRealtime(autoMoveInterval);

            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
        }
    }

    void IndicateMenuButton(int index)
    {
        var btnRect = buttons[index].GetComponent<RectTransform>();
        var pos = new Vector2(btnRect.localPosition.x, btnRect.localPosition.y);
        menuSelectIndicator.anchoredPosition = pos;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        InputController.ActiveInputMode = InputController.InputMode.Menu;

        menuSelector = StartCoroutine(MenuSelection());
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        InputController.ActiveInputMode = InputController.InputMode.Game;

        if (menuSelector != null)
        {
            StopCoroutine(menuSelector);
            menuSelector = null;
        }

    }

    public void RestartLevel()
    {
        if (Time.timeScale == 0)
        {
            PauseGame();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UserProgress.Current.SetField(new int[0]);
        return;
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        RestartLevel();
    }

    public void BackToMainMenu()
    {
        // TODO Load Main Menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        // Exit Game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("google.com");
#else
        Application.Quit();
#endif
    }
}