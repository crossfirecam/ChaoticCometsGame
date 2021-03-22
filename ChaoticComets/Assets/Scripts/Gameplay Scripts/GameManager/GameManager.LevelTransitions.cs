﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Start of Level Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // When a level starts, perform starting operations
    private IEnumerator StartNewLevel()
    {
        yield return new WaitForSeconds(0.05f);

        // Increase level count, and erase autosave data
        levelNo++;
        UiManager.i.SetWaveText(levelNo);
        Saving_SaveManager.EraseData();

        // Tell ships to disable model & colliders if they are dead
        if (player1dead)
            Refs.playerShip1.plrSpawnDeath.PretendShipDoesntExist(true);
        if (player2dead && BetweenScenes.PlayerCount == 2)
            Refs.playerShip2.plrSpawnDeath.PretendShipDoesntExist(true);

        // Asteroid number depends on level number. Iterated in SpawnProp()
        asteroidCount = 0;
        for (int i = 0; i < levelNo + 1; i++) { SpawnProp(PropType.Asteroid); }
        yield return new WaitForSeconds(0.01f);

        // Player Respawn
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.RespawnShip(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.RespawnShip(); }

        // Set a cap on how many UFOs can spawn. Double this in two-player. An aside... always one canteen per player per level.
        if (levelNo == 1) { ufoCap = 0; }
        else if (levelNo <= 3) { ufoCap = 1; }
        else if (levelNo <= 7) { ufoCap = 2; }
        else { ufoCap = 3; }

        // Double the cap if both players are alive
        if (!(player1dead || player2dead))
        {
            canisterCap *= 2; ufoCap *= 2;
        }

        // Set when the first UFO and canister will spawn
        AlienAndPowerupLogic(PropSpawnReason.AlienFirst);
        AlienAndPowerupLogic(PropSpawnReason.CanisterFirst);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * End of Level Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // At the end of a level activate level transition dialog, set the UFO to disappear, and if a player is still alive, refill their shields
    public void EndLevelFanFare()
    {
        if (!player1dead || !player2dead)
        {
            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<Ufo>().TeleportStart(true); }
            Invoke(nameof(EndLevelFanFare2), 2.5f);
        }
    }
    private void EndLevelFanFare2()
    {
        // Player Shield Recovery
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.ShipIsRecovering(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.ShipIsRecovering(); }
        UiManager.i.LevelCompleted();
        Invoke(nameof(BringUpShop), 3f);
    }

    // Leaves Main scene and brings up the shop
    private void BringUpShop()
    {
        BetweenScenes.ResumingFromSave = true;
        Saving_SaveManager.SaveData(this, Refs.playerShip1, Refs.playerShip2);
        SceneManager.LoadScene("ShopMenu");
    }

    // Alien UFO that spawns during the end-level transition will check this, and despawn if level transition is happening
    public bool CheckIfEndOfLevel()
    {
        if (asteroidCount <= 0 && !tutorialMode) { return true; }
        return false;
    }


}
