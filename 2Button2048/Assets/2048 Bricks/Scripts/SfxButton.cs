using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SfxButton : MonoBehaviour
{
    public Text label;

    public State sfxOn;
    public State sfxOff;

    [Serializable]
    public class State
    {
        public AudioMixerSnapshot snapshot;
        public string text;
    }

    static bool IsOn
    {
        get { return PlayerPrefs.GetInt("Sfx", 1) == 1; }
        set { PlayerPrefs.SetInt("Sfx", value ? 1 : 0); }
    }

    void Start()
    {
        UpdateState();
    }

    public void ChangeState()
    {
        IsOn = !IsOn;
        UpdateState();
    }

    void UpdateState()
    {
        State state = IsOn ? sfxOn : sfxOff;
        state.snapshot.TransitionTo(0f);
        label.text = state.text;
    }
}