﻿using System.Collections;
using UnityEngine;
using static Constants;

public partial class TutorialManager : MonoBehaviour
{

    private void ProgressCriteria()
    {
        switch (popUpIndex)
        {
            case 0: // Rotation
                ContinueIf(-player.GetAxis("Rotate") != 0);
                break;

            case 1: // Thrusting
                ContinueIf(player.GetAxis("Move") > 0);
                break;

            case 2: // Braking
                ContinueIf(player.GetAxis("Move") < 0);
                break;

            case 3: // Shooting
                ContinueIf(player.GetButtonDown("Shoot"));
                break;

            case 4: // Asteroids

                if (!taskSetupDone)
                {
                    GameManager.i.SpawnProp(GameManager.PropType.Asteroid, default, true); // Spawn safe asteroid
                    player1.collisionsCanDamage = false;
                    taskSetupDone = true;
                }
                ContinueIf(GameManager.i.asteroidCount > 1);
                break;

            case 5: // Asteroids 2
                ContinueIf(GameManager.i.asteroidCount == 0 || player.GetButtonDown("Teleport"));
                break;

            case 6: // Shields
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = true;
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(false);
                ContinueIf(player1.shields == 0);
                break;

            case 7: // Lives
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    taskSetupDone = true;
                }
                ContinueIf(player1.shields == 80 && player.GetButtonDown("Teleport"));
                break;

            case 8: // Canteen
                if (!taskSetupDone)
                {
                    GameManager.i.SpawnProp(GameManager.PropType.Canister);
                    taskSetupDone = true;
                }
                ContinueIf(player1.plrPowerups.ifTripleShot);
                break;

            case 9: // Powerup TripleShot
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = false;
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.TripleShot); // In case debugging sets popUpIndex to 9
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(player.GetButtonDown("Teleport"));
                break;

            case 10: // Powerup FarShot
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.TripleShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot);
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(player.GetButtonDown("Teleport"));
                break;

            case 11: // Powerup Auto-Brake
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.FarShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake);
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(player.GetButtonDown("Teleport"));
                break;

            case 12: // UFO Red
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = true;
                    playerCreditsBefore = player1.credits;
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.AutoBrake);
                    GameManager.i.SpawnProp(GameManager.PropType.UfoPasser);
                    taskSetupDone = true;
                }
                if (player1.credits > playerCreditsBefore && !ufoHit)
                {
                    ufoHit = true;
                    UfoPasser currentUfo = FindObjectOfType<UfoPasser>().GetComponent<UfoPasser>();
                    currentUfo.TeleportStart();
                }
                ContinueIf(ufoGone);
                break;

            case 13: // UFO Green
                if (!taskSetupDone)
                {
                    ufoHit = false; ufoGone = false; ufoFollowerDocile = true;
                    GameManager.i.SpawnProp(GameManager.PropType.UfoFollower);
                    ufoFollower = FindObjectOfType<UfoFollower>();
                    taskSetupDone = true;
                }
                if (!ufoFollower) // If player dies to passive UFO, replace the UFO when it teleports after player death
                {
                    GameManager.i.SpawnProp(GameManager.PropType.UfoFollower);
                    ufoFollower = FindObjectOfType<UfoFollower>();
                }
                ContinueIf(player.GetButtonDown("Teleport"));
                break;

            case 14: // Powerup RapidShot
                if (!taskSetupDone)
                {
                    StartCoroutine(ReplaceUfoFollower());
                    taskSetupDone = true;
                }
                if (!ufoFollower && !ufoFollowerDocile) // If player dies after aggressive UFO spawns in, replace the UFO when it teleports after player death
                {
                    GameManager.i.SpawnProp(GameManager.PropType.UfoFollower);
                    ufoFollower = FindObjectOfType<UfoFollower>();
                }
                ContinueIf(ufoFollower.alienHealth < 20);
                break;

            case 15: // Retreat
                if (!ufoFollower)
                {
                    StartCoroutine(ReplaceUfoFollower());
                }
                ContinueIf(ufoFollower.deathStarted || player.GetButtonDown("Teleport")); // Allow skipping
                break;
            case 16: // Powerup Insurance
                // This could be skipped instantly if player is 0 shields.
                // A separate bool and while loop were added to prevent this step being instantly skipped.
                if (!taskSetupDone)
                {
                    StartCoroutine(SetUpPopup16());
                    taskSetupDone = true;
                    player1.collisionsCanDamage = true;
                }
                if (player1.plrPowerups.ifInsurance)
                {
                    ResetAsteroidsIfZero(false);
                }
                ContinueIf(player1.shields == 0 && popup16Ready);
                break;

            case 17: // Teleport
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    taskSetupDone = true;
                }
                ContinueIf(player.GetButtonDown("Teleport") && player1.power == 80);
                break;

            case 18: // End

                break;

        }
    }

    private void ContinueIf(bool continuationCriteria)
    {
        if (continuationCriteria && !UiManager.i.GameIsPaused())
        {
            popups[popUpIndex].SetActive(false);
            popUpIndex++;
            popups[popUpIndex].SetActive(true);
            ChangePopupController();
            taskSetupDone = false;
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Tutorial prop management
     * ------------------------------------------------------------------------------------------------------------------ */

    private void CreateAsteroids(int num, bool safeAsteroids)
    {
        for (int i = 0; i < num; i++)
        {
            if (!safeAsteroids)
            {
                GameManager.i.SpawnProp(GameManager.PropType.Asteroid);
            }
            else
            {
                GameManager.i.SpawnProp(GameManager.PropType.Asteroid, default, true);
            }
        }
    }

    private void ResetAsteroidsIfZero(bool safeAsteroids)
    {
        if (GameManager.i.asteroidCount == 0)
        {
            CreateAsteroids(3, safeAsteroids);
        }
    }
    private void DestroyAllAsteroids()
    {
        GameObject[] listOfAsteroids = GameObject.FindGameObjectsWithTag(Tag_Other_AsteroidParent);
        foreach (GameObject asteroid in listOfAsteroids)
        {
            Destroy(asteroid.gameObject);
        }
        GameManager.i.asteroidCount = 0;
    }

    private IEnumerator ReplaceUfoFollower()
    {
        if (ufoFollower)
        {
            ufoFollower.TeleportStart(true);
            yield return new WaitForSeconds(3);
        }
        GameManager.i.SpawnProp(GameManager.PropType.UfoFollower);
        ufoFollower = FindObjectOfType<UfoFollower>();
        ufoFollowerDocile = false;

        if (!player1.plrPowerups.ifRapidShot)
            player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot);

        taskSetupDone = true;
    }

    private IEnumerator SetUpPopup16()
    {
        if (!ufoFollower.deathStarted)
            ufoFollower.TeleportStart(true); ufoFollowerDocile = true;
        yield return new WaitForSeconds(3);
        while (player1.shields == 0)
        {
            yield return new WaitForSeconds(1);
            print("Player is dead entering Popup16. Waiting for 1 second to apply Insurance");
        }
        popup16Ready = true;
        player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.Insurance);
        player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot);
        player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot);
        player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.TripleShot);
        player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.AutoBrake);
        CreateAsteroids(3, false);
    }
}
