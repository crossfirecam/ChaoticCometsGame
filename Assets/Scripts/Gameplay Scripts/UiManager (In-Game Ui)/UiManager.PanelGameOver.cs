﻿using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public partial class UiManager : MonoBehaviour
{
    [Header("Game Over Dialog UI")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private Button buttonWhenGameOver;
    [SerializeField] private TMP_InputField currentNameField;
    [SerializeField] private TextMeshProUGUI currentNameFieldPlaceholder;
    private int totalScore;
    private string mode;

    // Show game over panel and pause the game when the game is over
    public void GameOver()
    {
        // Bring cursor back, tell game not to attempt resuming from save if 'Play Again' is picked, and open panel
        BetweenScenes.ResumingFromSave = false;
        panelGameOver.SetActive(true);
        buttonWhenGameOver.Select();
        LoadLastUsedName();

        CalculateTotalScore("GameOver");

        // Shrink layout if a new high score is not accomplished
        if (!HighScoreHandling.IsThisAHighScore(totalScore))
        {
            RectTransform gameOverRt = panelGameOver.GetComponent<RectTransform>();
            gameOverRt.sizeDelta = new Vector2(gameOverRt.sizeDelta.x, 150);
            gameOverRt.Find("NewScoreParts").gameObject.SetActive(false);
        }


        // Halt all sounds and game speed
        MusicManager.i.PauseMusic();
        MusicManager.i.FindAllSfxAndPlayPause(0);
        Time.timeScale = 0;
    }

    // Reload the scene and restart playback if user decides to play again
    public void PlayAgain()
    {
        CheckAndSaveHighscore();
        UsefulFunctions.ResetBetweenScenesScript();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }

    // Saves highscore if it's accomplished, and returns to main menu.
    public void ExitGameFromGameOver()
    {
        CheckAndSaveHighscore();
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }

    // Return to main menu if user decides to leave from pause. Prompts user before leaving if a high score is accomplished.
    public void ExitGameFromPause()
    {
        CalculateTotalScore("MissionCancel");
        if (HighScoreHandling.IsThisAHighScore(totalScore) && !GameManager.i.tutorialMode)
        {
            panelPauseMenu.SetActive(false);
            panelGameOver.SetActive(true);
            fadeBlackOverlay.gameObject.SetActive(false);
            buttonWhenGameOver.Select();
            LoadLastUsedName();
        }
        else
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("StartMenu");
        }
    }

    private void CheckAndSaveHighscore()
    {
        // Set highscore's name depending on which panel's InputField is being used.
        string nameFromField;
        nameFromField = currentNameField.text;

        // Renames blank names to Anonymous
        if (string.IsNullOrWhiteSpace(nameFromField))
        {
            nameFromField = "Anonymous";
        }

        // Submit high score
        if (HighScoreHandling.IsThisAHighScore(totalScore))
        {
            HighScoreHandling.AddHighscoreEntry(nameFromField, GameManager.i.levelNo, totalScore, mode);

            // If name is not the default name, then set the preset name for next game in that mode. Reduces annoyance to controller users in particular.
            if (nameFromField == "Anonymous")
            {
                nameFromField = "";
            }
            if (BetweenScenes.PlayerCount == 1) { PlayerPrefs.SetString("SavedNameFor1P", nameFromField); }
            else if (BetweenScenes.PlayerCount == 2) { PlayerPrefs.SetString("SavedNameFor2P", nameFromField); }
        }
    }

    // Calculate total score, using Total Credits count to include upgrade spending. Update some text if there's two players.
    private void CalculateTotalScore(string originOfRequest)
    {
        string difficulty = "";
        switch (BetweenScenes.Difficulty)
        {
            case 1: difficulty = "Normal"; break;
            case 2: difficulty = "Hard"; break;
        }
        totalScore = GameManager.i.Refs.playerShip1.totalCredits;
        mode = $"1P ({difficulty})";
        if (BetweenScenes.PlayerCount == 2)
        {
            totalScore += GameManager.i.Refs.playerShip2.totalCredits;
            mode = $"2P ({difficulty})";
            currentNameFieldPlaceholder.text = "Enter names...";
        }
    }

    private void LoadLastUsedName()
    {
        currentNameField.text = PlayerPrefs.GetString(BetweenScenes.PlayerCount == 1 ? "SavedNameFor1P" : "SavedNameFor2P");
    }
}
