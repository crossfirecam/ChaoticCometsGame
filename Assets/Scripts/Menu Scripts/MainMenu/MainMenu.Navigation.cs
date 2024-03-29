﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class MainMenu : MonoBehaviour
{

    [Header("Main Menu UI Nav")]
    [SerializeField] private Transform mainMenuPanel;
    [SerializeField] private GameObject parsecDisclaimer;
    public OptionsPanel optionsPanel;
    public SavePanel savePanel;
    public ResetScoresPanel resetScoresPanel;
    private Button returnToMenuButton;

    /// <summary>
    /// <br>Checks if a save exists in Saving_SaveManager, which also isn't a save in Cheat Mode.</br>
    /// <br>If a save exists, show SavePanel. If not, immediately start the game.</br>
    /// </summary>
    /// <param name="plrAmountRequested">Set by the 'One' or 'Two Player' buttons on main menu.</param>
    public void CheckForSaveFile(int plrAmountRequested)
    {
        BetweenScenes.PlayerCount = plrAmountRequested;

        // If save found, and cheat mode flag is not found in the save, show save prompt
        // If none found, start the game
        if (Saving_SaveManager.LoadData() != null && !Saving_SaveManager.LoadData().isCheatModeOn)
        {
            savePanel.gameObject.SetActive(true);
            savePanel.ShowSavePanel();
        }
        else
        {
            StartGame();
        }
    }
    /// <summary>
    /// If the screen's fade effect is not playing, send the user back to the main menu, cancelling all panels.
    /// </summary>
    public void BackToMenu()
    {
        if (!fadeBlackOverlay.gameObject.activeInHierarchy)
        {
            audioMenuBack.Play();
            savePanel.gameObject.SetActive(false);
            resetScoresPanel.gameObject.SetActive(false);
            optionsPanel.gameObject.SetActive(false);
            parsecDisclaimer.SetActive(false);

            mainMenuPanel.gameObject.SetActive(true);
            returnToMenuButton.Select();
            returnToMenuButton = null;
        }
    }

    public void NotifyMainMenuWhichButtonToReturnTo()
    {
        if (returnToMenuButton == null)
        {
            returnToMenuButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
    }
}
