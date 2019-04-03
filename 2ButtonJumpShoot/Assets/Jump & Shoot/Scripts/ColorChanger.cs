using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public static float hueValue;

    public static void SetRandomBackgroundColor()
    {
        hueValue = Random.Range(0, 10) / 10.0f;
        ChangeBackgroundColor();
    }

    public static void ChangeBackgroundColor()
    {
        Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.6f, 0.8f);
        hueValue += 0.1f;

        if (hueValue >= 1)
        {
            hueValue = 0;
        }
    }
}
