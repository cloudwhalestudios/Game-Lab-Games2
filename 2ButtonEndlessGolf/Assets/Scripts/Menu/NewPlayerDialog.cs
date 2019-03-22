using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayerDialog : MonoBehaviour
{
    public Text nameText;
    public Text primaryKeyText;
    public Text promptText;

    public string defaultText = "Waiting for secondary button...";

    public void ShowDialog(string playerName, string primaryKey, Color color)
    {
        promptText.text = defaultText;
        nameText.text = playerName;
        nameText.color = color;
        primaryKeyText.text = primaryKey;

        gameObject.SetActive(true);
    }

    public void HideDialog()
    {
        gameObject.SetActive(false);

    }

    public void DisplayKeyInUse(string secondaryKey)
    {
        promptText.text = $"Button '{secondaryKey}' is already in use!\nPlease choose another...";
    }
}
