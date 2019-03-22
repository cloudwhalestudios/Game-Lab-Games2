using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectLevelController : LevelController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.StartLevelSelect();
    }
}
