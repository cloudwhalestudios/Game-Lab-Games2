using PlayerPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserProgress
{
    static UserProgress current;

    [SerializeField] int topScore;

    [SerializeField] KeyCode primary;
    [SerializeField] KeyCode secondary;

    public static UserProgress Current
    {
        get
        {
            if (current != null)
                return current;

            current = PlayerPreferenceManager.Load<UserProgress>();

            return current;
        }
    }

    public int TopScore { get => topScore; set { topScore = value; Save(); } }
    public KeyCode Primary { get => primary; set { primary = value; Save(); } }
    public KeyCode Secondary { get => secondary; set { secondary = value; Save(); } }

    public static void Save()
    {
        PlayerPreferenceManager.Save(current);
    }
}
