using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music input")]
    public AudioSource gameMusic;
    public AudioSource gameMusicFiltered;

    [Header("SFX input")]
    public AudioSource Death;
    public AudioSource Jump;
    public AudioSource Bounce;
    public AudioSource Fly;
    public AudioSource Shoot;
    public AudioSource Bass;
    public AudioSource BassCrash;
    public AudioSource BassCrashHighscore;
    public AudioSource Highscore;
    public AudioSource Alternate;
    public AudioSource Splash;
    public AudioSource UI;
    public AudioSource UI_cancel;
    public AudioSource UI_confirm;

    [Header("Sound effects")]
    public float lowPitchRange = 0.75f;
    public float highPitchRange = 1.25f;
    public float volumeValue;

    private bool filterSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        DisableMusic();
    }

    void Update()
    {
        if (GameManager.Instance?.isDead == false)
        {
            gameMusic.mute = false;
            gameMusicFiltered.mute = true;
        }
        else
        {
            gameMusic.mute = true;
            gameMusicFiltered.mute = false;
        }
    }

    private void DisableMusic()
    {
        gameMusic.mute = false;
        gameMusicFiltered.mute = false;
    }

    public void PlaySound(AudioSource sound)
    {
        sound.pitch = Random.Range(lowPitchRange, highPitchRange);
        sound.Play(0);
    }

    public void PlaySoundNormally(AudioSource sound)
    {
        sound.Play(0);
    }

}
