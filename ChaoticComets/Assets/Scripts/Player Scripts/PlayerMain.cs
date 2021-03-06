﻿using UnityEngine;
using static Constants;

/*
 * This class contains all general code for the Player objects, referring to many other scripts for extra functionality.
 */

public class PlayerMain : MonoBehaviour {

    [Header("Player Statistics")]
    public int playerNumber;
    public int credits = 0, totalCredits = 0, bonus = 9999;
    public float shields = 80, power = 0;

    [Header("Impacts, Death, Respawning")]
    private float nextDamagePossible = 0.0F;
    [SerializeField] internal bool collisionsCanDamage;
    internal float highDamageThreshold = 6f;
    private const float minTimeBetweenDamage = 0.05f;
    internal float damageFromImpact = 30f;
    internal float damageFromUfoBullet = 10f;

    [Header("Player Scripts")]
    internal PlayerInput plrInput = default;
    internal PlayerMovement plrMovement = default;
    internal PlayerPowerups plrPowerups = default;
    internal PlayerWeapons plrWeapons = default;
    internal PlayerMisc plrMisc = default;
    internal PlayerAbility plrAbility = default;
    internal PlayerSpawnDeath plrSpawnDeath = default;
    internal PlayerUiSounds plrUiSound = default;

    [Header("References")]
    public GameObject deathExplosion;
    public GameObject modelPlayer;
    internal Rigidbody2D rbPlayer;
    internal CapsuleCollider2D capsCollider;

    // ----------

    private void Awake() {
        GetComponents();
    }

    private void Start()
    {
        plrMisc.OtherStartFunctions();
    }

    // If game is not paused, then run per-frame updates
    private void Update() {
        if (!UiManager.i.GameIsPaused()) {
            plrInput.CheckInputs();
            plrMovement.ShipMovement();
            GameManager.i.CheckScreenWrap(transform);
            UiManager.i.SetPlayerStatusBars(playerNumber, shields, power);
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    public void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        totalCredits += pointsToAdd;
        plrUiSound.UpdatePointDisplays();
    }

    // When ship collides with asteroid or ufo colliders
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag(Tag_Asteroid) || col.gameObject.CompareTag(Tag_Ufo)) {


            if (collisionsCanDamage && Time.time > nextDamagePossible) {
                nextDamagePossible = Time.time + minTimeBetweenDamage;

                if (col.gameObject.CompareTag(Tag_Asteroid))
                    col.gameObject.GetComponent<AsteroidBehaviour>().AsteroidWasHit();

                plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactSoft;
                shields -= damageFromImpact;
                if (shields <= 0)
                {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxDeath;
                    plrSpawnDeath.ShipIsDead();
                }
                plrUiSound.audioShipSFX.Play();
            }
            else if (plrAbility.teleportOut.gameObject.activeInHierarchy)
            {
                if (col.gameObject.CompareTag(Tag_Asteroid)) { col.gameObject.GetComponent<AsteroidBehaviour>().AsteroidWasHit(); }
            }
        }
    }

    // When ship collides with alien bullet or powerup triggers
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.CompareTag(Tag_BulletUfoGreen) || triggerObject.gameObject.CompareTag(Tag_BulletUfoRed))
        {
            if (collisionsCanDamage && Time.time > nextDamagePossible) {
                Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
                triggerObject.GetComponent<CircleCollider2D>().enabled = false;
                Destroy(triggerObject.gameObject, 5f);

                if (triggerObject.gameObject.CompareTag(Tag_BulletUfoGreen))
                    shields -= damageFromUfoBullet;
                else // UFO-Passer
                    shields -= damageFromUfoBullet * 2f;
                nextDamagePossible = Time.time + minTimeBetweenDamage;
                plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactSoft;

                if (shields <= 0)
                {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxDeath;
                    plrSpawnDeath.ShipIsDead();
                }
                plrUiSound.audioShipSFX.Play();
            }
        }
        if (triggerObject.gameObject.CompareTag(Tag_Canister) && modelPlayer.activeInHierarchy) {
            Destroy(triggerObject.gameObject);
            plrPowerups.GivePowerup();
        }
    }

    private void GetComponents()
    {
        rbPlayer = gameObject.GetComponent<Rigidbody2D>();
        capsCollider = gameObject.GetComponent<CapsuleCollider2D>();
        plrInput = gameObject.GetComponent<PlayerInput>();
        plrMovement = gameObject.GetComponent<PlayerMovement>();
        plrPowerups = gameObject.GetComponent<PlayerPowerups>();
        plrWeapons = gameObject.GetComponent<PlayerWeapons>();
        plrMisc = gameObject.GetComponent<PlayerMisc>();
        plrAbility = gameObject.GetComponent<PlayerAbility>();
        plrSpawnDeath = gameObject.GetComponent<PlayerSpawnDeath>();
        plrUiSound = gameObject.GetComponent<PlayerUiSounds>();
    }
}
