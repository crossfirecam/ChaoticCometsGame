﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadScene", 1f);
    }

    private void LoadScene()
    {
        // If control panel is called while resuming a save, then load store before next level
        // Else, load as if a new game was started
        if (BetweenScenesScript.ResumingFromSave == true)
        {
            SceneManager.LoadScene("ShopMenu");
        }
        else
        { // If a new game is started, then erase old data
            Saving_SaveManager.EraseData();
            SceneManager.LoadScene("MainScene");
        }
    }

    public void VisitTutorial()
    {
        BetweenScenesScript.TutorialMode = true;
        SceneManager.LoadScene("MainScene");
    }
    public void VisitHelp()
    {
        SceneManager.LoadScene("HelpMenu");
    }

    public void VisitAbout()
    {
        SceneManager.LoadScene("AboutMenu");
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
