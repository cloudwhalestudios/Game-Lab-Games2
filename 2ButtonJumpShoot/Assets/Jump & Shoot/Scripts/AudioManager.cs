using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public GameManager theGameManager;

    [Header("Music input")]
    public AudioSource gameMusic;
    public AudioSource gameMusicFiltered;

    [Header("Sound effects")]
    public float lowPitchRange = 0.75f;
    public float highPitchRange = 1.25f;
    public float volumeValue;

    private bool filterSound;

    void Start()
    {
        theGameManager = FindObjectOfType<GameManager>();
        DisableMusic();
    }

    void Update()
    {
        if (theGameManager.isDead == false)
        {
            gameMusic.mute = false;
            gameMusicFiltered.mute = true;
        } else
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

}
