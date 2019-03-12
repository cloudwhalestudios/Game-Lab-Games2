using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreCounter : MonoBehaviour
{
    Text label;

    protected virtual int Value
    {
        get { return UserProgress.Current.Score; }
    }

    void Start()
    {
        label = GetComponent<Text>();

        OnProgressUpdate();
        UserProgress.Current.ProgressUpdate += OnProgressUpdate;
    }

    void OnDestroy()
    {
        UserProgress.Current.ProgressUpdate -= OnProgressUpdate;
    }

    void OnProgressUpdate()
    {
        label.text = Value.ToString();
    }
}