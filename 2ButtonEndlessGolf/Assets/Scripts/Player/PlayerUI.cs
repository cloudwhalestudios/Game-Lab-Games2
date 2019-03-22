using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text nameText;
    public Text primary;
    public Text secondary;

    public void ShowKeys(bool show)
    {
        primary.gameObject.SetActive(show);
        secondary.gameObject.SetActive(show);
    }

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    internal void Init(string name, KeyCode primaryKey, KeyCode secondaryKey)
    {
        nameText.text = name;
        primary.text = primaryKey.ToString();
        secondary.text = secondaryKey.ToString();
    }
}
