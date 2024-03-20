using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Player.Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinGame : MonoBehaviour
{
    public bool isServer;
    public void Join()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux || isServer)
        {
            NetworkManager.Singleton.StartServer();
        }
        /*else if (Application.isEditor)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Start host");
            CreatePlayer();
        }*/
        else if(Application.isPlaying){
            NetworkManager.Singleton.StartClient();
            Debug.Log("Start client");
            CreatePlayer();
        }
    }

    void CreatePlayer()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")
            .First(player => player.GetComponent<NetworkObject>().IsOwner);

        var playerData = ScriptableObject.CreateInstance<PlayerData>();
        playerData.playerNameLabel = player.GetComponentInChildren<TextMeshProUGUI>();
            
        playerData.playerName = $"player{PlayerManager.players.Count}";
        playerData.currentHealth = 100f;
        playerData.maxHealth = 100f;
        playerData.defaultMovementSpeed = 0.5f;
        playerData.maxStamina = 100f;
        playerData.currentStamina = 100f;
        playerData.staminaRegenRate = 10f;
        
        PlayerManager.spawnPlayer(player, playerData);
    }
    
}
