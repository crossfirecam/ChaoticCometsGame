﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * This class handles all in-game meta logic, such as level transitions, telling objects when and how to spawn, player management, etc.
 */

public partial class GameManager : MonoBehaviour
{

    [Header("Tutorial Mode")]
    public bool tutorialMode = true; // Not in tutorial by default

    [Header("General purpose variables")]
    public int asteroidCount;
    public int levelNo = 0;
    [HideInInspector] public float screenTop = 8f, screenBottom = 8f, screenLeft = 11f, screenRight = 11f;

    [Header("Other Variables")]
    private AudioSource musicLoop;
    private float fadingAlpha = 0f;

    [Header("Inspector References")]
    public GameManagerHiddenVars Refs;

    void Start() {
        Cursor.visible = false;
        musicLoop = gameObject.GetComponent<AudioSource>();
        // If in tutorial mode, activate TutorialManager
        if (BetweenScenesScript.TutorialMode || tutorialMode)
        {
            Refs.tutorialManager.SetActive(true);
            tutorialMode = true;
        }
        // If in normal gameplay, set player ships to become active and start gameplay
        else
        {
            if (BetweenScenesScript.MusicVolume > 0f)
            {
                musicLoop.Play();
            }
            if (BetweenScenesScript.PlayerCount == 2) {
                Refs.player2GUI.SetActive(true);
                Refs.playerShip2.gameObject.SetActive(true);
                player2dead = false;
            }
            if (BetweenScenesScript.ResumingFromSave) { // If resuming from save file, read from save file first
                Saving_PlayerManager data = Saving_SaveManager.LoadData();
                levelNo = data.level;
                // Tell ships to 'play dead' (disable model & colliders) if previous shop says they're dead
                if (BetweenScenesScript.player1TempLives == 0)
                {
                    Refs.playerShip1.plrSpawnDeath.PretendShipDoesntExist();
                }
                if (BetweenScenesScript.player2TempLives == 0 && BetweenScenesScript.PlayerCount == 2)
                {
                    Refs.playerShip2.plrSpawnDeath.PretendShipDoesntExist();
                
                }
            }
            StartCoroutine(FadeBlack("from"));
            StartCoroutine(StartNewLevel());
        }
    }

    void Update() {
        // Each frame, check if pause menu is open, and what button is highlighted.
        // If the mouse is used to click auto highlight away, then drag a highlight back onto a certain button.
        if (Refs.gamePausePanel.activeInHierarchy || Refs.gameOverPanel.activeInHierarchy) {
            if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.Equals(null))
            {
                // If on pause panel, then select the resume button. If on game over panel, select play again button.
                if (Refs.gamePausePanel.activeInHierarchy) { Refs.buttonWhenPaused.Select(); }
                else { Refs.buttonWhenGameOver.Select(); }
            }
        }

        // Check if pause button is pressed, and resume/pause game.
        if (Input.GetButtonDown("Pause") && !Refs.gameOverPanel.activeInHierarchy) {
            if (Refs.gamePausePanel.activeInHierarchy) {
                PauseGame(1);
            }
            else {
                PauseGame(0);
            }
        }
    }

    // Screen Wrapping
    public void CheckScreenWrap(Transform current, float xOldOffset = 0f, float yOldOffset = 0f, float xNewOffset = 0f, float yNewOffset = 0f)
    {
        Vector2 newPosition = current.position;
        if (current.position.y > screenTop - yOldOffset) { newPosition.y = screenBottom + yNewOffset; }
        if (current.position.y < screenBottom + yOldOffset) { newPosition.y = screenTop - yNewOffset; }
        if (current.position.x > screenRight - xOldOffset) { newPosition.x = screenLeft + xNewOffset; }
        if (current.position.x < screenLeft + xOldOffset) { newPosition.x = screenRight - xNewOffset; }
        current.position = newPosition;
    }
}

[System.Serializable]
public class GameManagerHiddenVars
{
    public GameObject tutorialManager;

    [Header("UI References")]
    public GameObject fadeBlack, player2GUI;
    public GameObject gameOverPanel, gamePausePanel, gameLevelPanel;
    public GameObject gameLevelShieldRechargeText;
    public Text swapP1Text, swapP2Text;
    public Button buttonWhenPaused, buttonWhenGameOver, buttonWhenLeavingPauseBugFix;

    [Header("Prop References")]
    public PlayerMain playerShip1;
    public PlayerMain playerShip2;
    public GameObject largeAsteroidProp, ufoFollowerProp, ufoPasserProp, canisterProp;
    public GameObject largeAsteroidSafeProp;
}