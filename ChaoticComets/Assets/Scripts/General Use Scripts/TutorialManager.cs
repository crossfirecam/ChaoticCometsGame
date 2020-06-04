﻿using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

public partial class TutorialManager : MonoBehaviour
{
    public int popUpIndex = -1;
    private GameManager gM;
    public PlayerMain player1;
    private UfoFollower ufoFollower;
    public GameObject[] popups;
    private bool taskSetupDone = false, ufoHit = false;
    public bool ufoGone = false, ufoFollowerDocile = false;
    private int playerCreditsBefore = 0;
    public enum ControlType { KeyboardP1, KeyboardP2, Xbox }
    public ControlType chosenControlStyle = ControlType.KeyboardP1;

    private Player player;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
        gM = FindObjectOfType<GameManager>();
        player1.power = 0;
        player1.plrUiSound.UpdatePointDisplays();

        if (popUpIndex == -1) // Set to -1 only when tutorial starts. Does not apply when debugging.
            DisplayChoiceDialog();
        else
        {
            popups[popUpIndex].SetActive(true);
            ChangePopupController();
        }

    }

    private void Update()
    {
        ProgressCriteria();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Popup string editing
     * ------------------------------------------------------------------------------------------------------------------ */

    // Replace a placeholder value with the corresponding control style's key prompts. Find a better way to do this TODO
    //                                         Rotate                            Thrust       Brake        Ability      Shoot        Pause
    private readonly string[] replaceOrig = {  "R_Rotate",                       "R_Thrust",  "R_Brake",   "R_Ability", "R_Shoot",   "R_Pause" };
    private readonly string[] replaceP1Key = { "Press <sprite=1> or <sprite=3>", "sprite=0",  "sprite=2",  "sprite=4",  "sprite=5",  "sprite=12" };
    private readonly string[] replaceP2Key = { "Press <sprite=7> or <sprite=9>", "sprite=6",  "sprite=8",  "sprite=10", "sprite=11", "sprite=12" };
    private readonly string[] replaceXbox =  { "Tilt <sprite=13>",               "sprite=14", "sprite=15", "sprite=16", "sprite=17", "sprite=18", };
    private void ChangePopupController()
    {
        string[] replaceChosen = default;
        switch (chosenControlStyle)
        {
            case ControlType.KeyboardP1:
                replaceChosen = replaceP1Key;
                break;
            case ControlType.KeyboardP2:
                replaceChosen = replaceP2Key;
                break;
            case ControlType.Xbox:
                replaceChosen = replaceXbox;
                break;
        }

        StringBuilder editedPopup = new StringBuilder();
        editedPopup.Append(popups[popUpIndex].GetComponent<TextMeshProUGUI>().text);

        for (int i = 0; i < replaceOrig.Length; i++)
        {
            editedPopup.Replace(replaceOrig[i], replaceChosen[i]);
        }
        popups[popUpIndex].GetComponent<TextMeshProUGUI>().text = editedPopup.ToString();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Choice of control scheme
     * ------------------------------------------------------------------------------------------------------------------ */

    private void DisplayChoiceDialog()
    {
        Cursor.visible = true;
        Time.timeScale = 0;
        gM.Refs.tutorialChoicePanel.SetActive(true);
        gM.Refs.buttonWhenTutorialChoice.Select();
    }

    public void SetControls(string chosenControl)
    {
        switch (chosenControl)
        {
            case "KeyboardP1":
                chosenControlStyle = ControlType.KeyboardP1;
                break;
            case "KeyboardP2":
                chosenControlStyle = ControlType.KeyboardP2;
                player1.GetComponent<PlayerInput>().SwapToP2InputForTutorial();
                player = ReInput.players.GetPlayer(1);
                break;
            case "Xbox":
                chosenControlStyle = ControlType.Xbox;
                break;
        }

        gM.Refs.tutorialChoicePanel.SetActive(false);
        Time.timeScale = 1;
        popUpIndex++;
        popups[popUpIndex].SetActive(true);
        ChangePopupController();
    }
}
