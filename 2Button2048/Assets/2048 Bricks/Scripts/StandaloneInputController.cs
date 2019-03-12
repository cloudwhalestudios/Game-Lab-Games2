using UnityEngine;

public class StandaloneInputController : TwoButtonInputController
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnPrimary();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            OnSecondary();
    }
}