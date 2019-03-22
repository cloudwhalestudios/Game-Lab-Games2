using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenuController : MenuController
{
    public void Start()
    {
        MenuManager.Instance.SetActiveMenu(this);
        itemSelectIndicator.gameObject.SetActive(true);
    }
}
