using UnityEngine;

public class StandaloneInputController : InputController
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnPrimary();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            OnSecondary();
    }
}