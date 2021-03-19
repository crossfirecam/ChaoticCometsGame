﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private void CheckIfResumingFromSave()
    {
        if (BetweenScenes.ResumingFromSave)
        { // If resuming from save file, read from save file first
            Saving_PlayerManager data = Saving_SaveManager.LoadData();
            levelNo = data.level;
            playerLives = data.lives;
            player1dead = data.player1dead;
            player2dead = data.player2dead;
        }
    }
    private void PlayMusicIfEnabled()
    {
        // Check for MusicManager
        musicManager = FindObjectOfType<MusicManager>();
        if (!musicManager)
        {
            Instantiate(Refs.musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }

        // Change music track & play
        if (!tutorialMode)
            musicManager.ChangeMusicTrack(1);
        else
            musicManager.ChangeMusicTrack(3);
        if (PlayerPrefs.GetFloat("Music") > 0f)
            musicManager.currentMusicPlayer.Play();
    }

    // Auto-save warning for Pause Menu. Don't show if game just starting, it's level 1, or it's tutorial mode.
    public bool AutoSaveWarnShouldHide()
    {
        return levelNo == 0 || levelNo == 1 || tutorialMode;
    }
}