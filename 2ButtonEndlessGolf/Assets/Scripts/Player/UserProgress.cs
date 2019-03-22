using System;
using UnityEngine;

public class UserProgress
{
    static UserProgress current;

    public event Action ProgressUpdate;

    [SerializeField]
    int score;
    [SerializeField]
    int topScore;

    [SerializeField]
    int[] field = new int[0];

    [SerializeField]
    int currentBrickValue;
    [SerializeField]
    int nextBrick;

    public static UserProgress Current
    {
        get
        {
            if (current != null)
                return current;

            string progressJson = PlayerPrefs.GetString("UserProgress", "{}");
            Debug.Log("UserProgress : " + progressJson);
            current = JsonUtility.FromJson<UserProgress>(progressJson);

            return current;
        }
    }

    public int Score
    {
        get { return score; }
        set
        {
            score = value;

            if (score > topScore)
                topScore = score;

            if (ProgressUpdate != null)
                ProgressUpdate.Invoke();
        }
    }

    public int TopScore
    {
        get { return topScore; }
    }
    public int CurrentBrickValue
    {
        get { return currentBrickValue; }
        set
        {
            currentBrickValue = value;
        }
    }
    public int NextBrick
    {
        get { return nextBrick; }
        set
        {
            nextBrick = value;
        }
    }

    public int[] GetField()
    {
        return (int[]) field.Clone();
    }

    public void SetField(int[] value)
    {
        field = (int[]) value.Clone();
    }

    public void Save()
    {
        string progressJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("UserProgress", progressJson);
    }
}