using UnityEngine;

public class StandaloneInputController : InputController
{
    public override void Init(KeyCode primary, KeyCode secondary, int id)
    {
        PlayerID = id;
        PrimaryKey = primary;
        SecondaryKey = secondary;
    }
}