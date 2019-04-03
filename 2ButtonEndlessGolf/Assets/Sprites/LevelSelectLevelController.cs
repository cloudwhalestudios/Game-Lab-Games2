using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectLevelController : LevelController
{

    [Header("Level Button Prefab")]
    public GameObject levelButtonPrefab;
    public Transform buttonParent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.StartLevelSelect();
        PlayerManager.Instance.SetPlayerActive(true);
        AddLevelButtons();
    }

    void AddLevelButtons()
    {
        var i = 1;
        foreach (var levelControllerPrefab in LevelManager.Instance.levelControllerPrefabs)
        {
            var levelButton = Instantiate(levelButtonPrefab, buttonParent);
            levelButton.name += " " + i;
            levelButton.GetComponentInChildren<Text>().text = i.ToString();
            levelButton.GetComponent<Button>().onClick.AddListener(() => LevelManager.Instance.LoadLevel(levelControllerPrefab));
            i++;
        }
    }
}
