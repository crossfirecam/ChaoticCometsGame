﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public partial class MainMenu : MonoBehaviour
{
    [Header("Main Menu Misc")]
    public AudioMixer mixer;
    public Image fadeBlackOverlay;
    private AudioSource audioMenuBack;

    // ----------
    private void Start()
    {
        StartupSoundManagement();
        UsefulFunctions.ResetBetweenScenesScript();
        ChangeScoreTypeAndPopulate(PlayerPrefs.GetInt("ScorePreference", 0));

        // Check most recently used controller every .2 seconds
        StartCoroutine(UsefulFunctions.CheckController());

        // If returning to main menu from About or Help, play a sound and select the button. If not, then fade the screen in.
        if (BetweenScenes.BackToMainMenuButton != "")
        {
            Button buttonToSelect = mainMenuPanel.Find(BetweenScenes.BackToMainMenuButton + "Button").GetComponent<Button>();
            buttonToSelect.Select();
            audioMenuBack.Play();
            BetweenScenes.BackToMainMenuButton = "";
        }
        else
        {
            StartCoroutine(UsefulFunctions.FadeScreenBlack("from", fadeBlackOverlay));
        }
    }

    private void StartupSoundManagement()
    {
        // Find Music Manager & audio source for moving back in Main Menu UI
        audioMenuBack = GetComponent<AudioSource>();

        // Find the SFX slider in Options, and set default game values (for some reason GetFloat's defaultValue wouldn't work...)
        MusicManager.i.sfxDemo = optionsPanel.optionSFXSlider.GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.8f);
            PlayerPrefs.SetFloat("SFX", 0.8f);
            PlayerPrefs.Save();
        }

        // Change music to main menu track, set volumes
        MusicManager.i.ChangeMusicTrack(0);
        float musicVol = PlayerPrefs.GetFloat("Music");
        float sfxVol = PlayerPrefs.GetFloat("SFX");
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }
}
