using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupLevelController : LevelController
{
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.StartMainMenu();
    }
}
