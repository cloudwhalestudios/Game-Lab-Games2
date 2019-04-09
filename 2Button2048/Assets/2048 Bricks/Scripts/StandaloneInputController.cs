using UnityEngine;

public class StandaloneInputController : InputController
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
            OnPrimary();

        if (Input.GetKeyDown(KeyCode.Keypad4))
            OnSecondary();
    }
}