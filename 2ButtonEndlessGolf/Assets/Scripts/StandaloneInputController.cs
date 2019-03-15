using UnityEngine;

public class StandaloneInputController : InputController
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            OnPrimary();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnSecondary();
    }
}