﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public partial class MainMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject fadeBlack;
    private float fadingAlpha = 0f;

    // ----------

    private void Start() {
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false; // Set to false first, in case game is closed while save is being loaded
        BetweenScenesScript.TutorialMode = false;
        BetweenScenesScript.MusicVolume = PlayerPrefs.GetFloat("Music");
        BetweenScenesScript.SFXVolume = PlayerPrefs.GetFloat("SFX");
        BetweenScenesScript.player1TempCredits = 0; // Reset temporary credit & lives count to 0. These will be set if a store is loaded and progressed past
        BetweenScenesScript.player2TempCredits = 0;
        BetweenScenesScript.player1TempLives = 0;
        BetweenScenesScript.player2TempLives = 0;
        mixer.SetFloat("MusicVolume", Mathf.Log10(BetweenScenesScript.MusicVolume) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(BetweenScenesScript.SFXVolume) * 20);
        StartCoroutine(FadeBlack("from"));
    }

    private void Update() {
        CheckForMenuNavCancel();
        CheckHighlightedButton();
        CheckForControllerOrKeyboard();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Other functions
     * ------------------------------------------------------------------------------------------------------------------ */

    private IEnumerator FadeBlack(string ToOrFrom) {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.8f;
        fadeBlack.SetActive(true);
        if (ToOrFrom == "from") {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f) {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
        }
        else if (ToOrFrom == "to") {
            fadingAlpha = 0f;
            speedOfFade = 1.6f;
            while (fadingAlpha < 1f) {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}