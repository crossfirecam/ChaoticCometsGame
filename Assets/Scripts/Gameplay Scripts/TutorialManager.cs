﻿using Rewired;
using System.Text;
using TMPro;
using UnityEngine;

public partial class TutorialManager : MonoBehaviour
{
    public enum ControlType { KeyboardUndecided, KeyboardP1, KeyboardP2, Xbox, PS, Switch }

    [Header("Tutorial Task Variables")]
    public int popUpIndex = -1;
    private bool taskSetupDone = false, ufoHit = false, popup16Ready = false;
    public bool ufoGone = false, ufoFollowerDocile = false;
    private int playerCreditsBefore = 0;
    public ControlType chosenControlStyle = ControlType.KeyboardUndecided;

    [Header("References")]
    private Player player;
    public PlayerMain player1;
    private UfoFollower ufoFollower;
    public GameObject[] popups;

    private static TutorialManager _i;
    public static TutorialManager i { get { if (_i == null) _i = FindObjectOfType<TutorialManager>(); return _i; } }
    [SerializeField] private TextMeshProUGUI tutorialIntroSubtitle;

    private void Start()
    {
        #if UNITY_WEBGL
            tutorialIntroSubtitle.text = "Tutorial won't be accurate because of <u>limited controller support</u> on the <u>web version</u>.";
        #endif

        player = ReInput.players.GetPlayer(0);
        player1.power = 0;
        player1.plrUiSound.UpdatePointDisplays();

        if (popUpIndex == -1) // Set to -1 only when tutorial starts. Does not apply when debugging.
            UiManager.i.DisplayTutorialChoiceDialog();
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

    // Replace a placeholder value with the corresponding control style's key prompts.
    //                                          Rotate                                      Thrust       Brake        Ability      Shoot        Pause
    private readonly string[] replaceOrig =   { "R_Rotate",                                 "R_Thrust",  "R_Brake",   "R_Ability", "R_Shoot",   "R_Pause" };
    private readonly string[] replaceP1Key =  { "Press <sprite=1> or <sprite=3>",           "sprite=0",  "sprite=2",  "sprite=4",  "sprite=5",  "sprite=12" };
    private readonly string[] replaceP2Key =  { "Press <sprite=7> or <sprite=9>",           "sprite=6",  "sprite=8",  "sprite=10", "sprite=11", "sprite=12" };
    private readonly string[] replaceXbox =   { "Tilt <sprite=19> <sprite=13> <sprite=20>", "sprite=14", "sprite=15", "sprite=16", "sprite=17", "sprite=18", };
    private readonly string[] replacePS =     { "Tilt <sprite=19> <sprite=13> <sprite=20>", "sprite=21", "sprite=22", "sprite=23", "sprite=24", "sprite=25", };
    private readonly string[] replaceSwitch = { "Tilt <sprite=19> <sprite=13> <sprite=20>", "sprite=26", "sprite=27", "sprite=16", "sprite=17", "sprite=28", };
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
            case ControlType.PS:
                replaceChosen = replacePS;
                break;
            case ControlType.Switch:
                replaceChosen = replaceSwitch;
                break;
        }

        StringBuilder editedPopup = new StringBuilder();
        editedPopup.Append(popups[popUpIndex].GetComponentInChildren<TextMeshProUGUI>().text);

        for (int i = 0; i < replaceOrig.Length; i++)
        {
            editedPopup.Replace(replaceOrig[i], replaceChosen[i]);
        }
        popups[popUpIndex].GetComponentInChildren<TextMeshProUGUI>().text = editedPopup.ToString();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Choice of control scheme
     * ------------------------------------------------------------------------------------------------------------------ */

    public void SetControls(string chosenControl)
    {
        switch (chosenControl)
        {
            case "Keyboard":
                chosenControlStyle = ControlType.KeyboardUndecided;
                break;
            case "Xbox":
                chosenControlStyle = ControlType.Xbox;
                break;
            case "PS":
                chosenControlStyle = ControlType.PS;
                break;
            case "Switch":
                chosenControlStyle = ControlType.Switch;
                break;
        }
        UiManager.i.DisplayTutorialPlayerDialog();
    }

    public void PlayerChosen(int playerId)
    {
        if (chosenControlStyle == ControlType.KeyboardUndecided)
        {
            if (playerId == 0)
                chosenControlStyle = ControlType.KeyboardP1;
            else
                chosenControlStyle = ControlType.KeyboardP2;
        }

        player = ReInput.players.GetPlayer(playerId);
        player1.GetComponent<PlayerInput>().SwapInputForTutorial(playerId);

        UiManager.i.DismissTutorialChoiceDialog();
        popUpIndex++;
        popups[popUpIndex].SetActive(true);
        ChangePopupController();
    }
}
