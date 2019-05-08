using UnityEngine;

public class StandaloneInputController : InputController
{
    [SerializeField] KeyCode primaryKey;
    [SerializeField] KeyCode secondaryKey;


    private void Start()
    {
        if (PlatformPreferences.Current?.Keys != null)
        {
            primaryKey = PlatformPreferences.Current.Keys[0];
            secondaryKey = PlatformPreferences.Current.Keys[1];
        }
        else if (primaryKey == KeyCode.None || secondaryKey == KeyCode.None)
        {
            primaryKey = KeyCode.LeftArrow;
            secondaryKey = KeyCode.RightArrow;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(primaryKey))
            OnPrimary();

        if (Input.GetKeyDown(secondaryKey))
            OnSecondary();
    }
}