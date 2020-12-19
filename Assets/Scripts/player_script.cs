﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class player_script : MonoBehaviour
{
    public entity_spawn_script entity_spawn_script;
    public projectile_script projectile_script;
    public global_script global_script;
    public mouse_script mouse_script;

    private float playerHealthTimeFloat;
    private int playerMaxHealth = 100;
    public int playerHealth = 100;
    private int playerHealthTime;

    private float rateOfFire = 0.120f;
    private float playerShotTime;
    public int playerAmmo = 25;

    private float playerMoveSpeed = 0.125f;

    public GameObject restartHint;

    void FixedUpdate()
    {
        mouse_script.MouseController();
        PlayerController();
        PlayerHealth();
        Projectile();
    }

    void Projectile()
    {
        LayerMask mask = LayerMask.GetMask("enemy");
        RaycastHit hit;

        playerShotTime += Time.deltaTime;

        if (Input.GetKey(KeyCode.Mouse0) && playerShotTime >= rateOfFire && playerAmmo > 0)
        {
            projectile_script.ProjectilePrefab();

            playerShotTime = 0;
            playerAmmo--;

            if (Physics.Raycast(global_script.player.transform.position, global_script.player.transform.forward, out hit, 50, mask))
            {
                Destroy(hit.transform.gameObject);

                entity_spawn_script.milestoneEnemyCount++;
                entity_spawn_script.currentEnemyCount--;
                global_script.playerScore += 50;
                playerAmmo += 2;
            }
        }
    }

    void PlayerController()
    {
        float keyboardX = Input.GetAxis("Horizontal");
        float keyboardY = Input.GetAxis("Vertical");

        global_script.mouseCursor.transform.Translate(keyboardX * playerMoveSpeed, 0, keyboardY * playerMoveSpeed, Space.World);
        global_script.player.transform.Translate(keyboardX * playerMoveSpeed, 0, keyboardY * playerMoveSpeed, Space.World);

        Vector3 lookAtPosition = new Vector3(global_script.mouseCursor.transform.position.x, global_script.player.transform.position.y, global_script.mouseCursor.transform.position.z);
        global_script.player.transform.LookAt(lookAtPosition);
    }

    void PlayerHealth()
    {
        if (playerHealth < playerMaxHealth)
        {
            playerHealthTime = Mathf.RoundToInt(playerHealthTimeFloat);
            playerHealthTimeFloat += 0.25f * Time.deltaTime;
            if (playerHealthTime >= 1)
            {
                playerHealth += 1;
                if (playerHealth >= playerMaxHealth)
                {
                    playerHealthTime = 0;
                }
            }
        }

        if (playerHealth <= 0)
        {
            global_script.mouseCursor.SetActive(false);
            global_script.player.SetActive(false);
            restartHint.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision Collision)
    {
        if (Collision.gameObject.tag == "enemy")
        {
            playerHealthTimeFloat = 0;
            playerHealth -= 50;
        }
    }
}