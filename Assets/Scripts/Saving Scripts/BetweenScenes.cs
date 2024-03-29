﻿public static class BetweenScenes {
    // Defaut value of 1 player for debugging. Alternative is '2'.
    public static int PlayerCount = 1;
    // Default value of Normal difficulty for debugging. Alternative is '2' Hard. (Deprecated values: 0, 3)
    public static int Difficulty = 1;

    // By default, the game assumes it's not being resumed from a save. Until it either...
    // A: New game is started, and first level is finished
    // B: Game is resumed from a found save file
    public static bool ResumingFromSave = false;

    // When navigating to 'Help', 'About', 'Tutorial' scenes, Main Menu uses this to remember which button to return selection to
    public static string BackToMainMenuButton = "";

    // Tutorial mode on or off
    public static bool TutorialMode = false;
    // Cheats been activated this round
    public static bool CheaterMode = false;

    // Upgrades for the players. These values are only committed to a save once the current shop and next area are passed.
    // In order, upgrades are: Shield Str, Teleport Rate, Ship Speed, Brake Power, Auto Firerate, Shot Limit, Shot Speed, Shot Range.
    public static int[][] PlayerShopUpgrades = new int[2][] {
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    // Credits and life counter are tracked here, because shop allows players to refund by leaving and coming back.
    // These values aren't committed to the save until the time the next level ends.
    public static int[] PlayerShopCredits = { 0, 0 };
    public static float[] PlayerShopShields = { 0, 0 };
    public static int PlayerShopLives = 0;
}
