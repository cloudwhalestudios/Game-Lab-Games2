using UnityEngine;

public class StandaloneInputController : InputController
{
    public override void Init(KeyCode primary, KeyCode secondary, int id)
    {
        PlayerID = id;
        PrimaryKey = primary;
        SecondaryKey = secondary;
    }

    void Update()
    {
        if (Input.GetKeyDown(PrimaryKey))
            OnPrimary(PrimaryKey);

        if (Input.GetKeyDown(SecondaryKey))
            OnSecondary(SecondaryKey);
    }
}