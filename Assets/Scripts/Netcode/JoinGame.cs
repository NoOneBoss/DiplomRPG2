using System;
using System.Collections;
using Configs;
using DavidFDev.DevConsole;
using Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class JoinGame : NetworkBehaviour
{
    public PlayerDataObject defaultData;
    private bool isSceneLoading = false;
    private void Start()
    {
        DontDestroyOnLoad(Camera.main);
        if (Application.isBatchMode)
        {
            NetworkManager.StartServer();
            loadScene("SampleScene", LoadSceneMode.Additive);
        }
    }

    private void loadScene(string name, LoadSceneMode mode)
    {
        if (NetworkManager.IsServer && !isSceneLoading)
        {
            isSceneLoading = true;
            var status = NetworkManager.SceneManager.LoadScene(name, mode);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load scene with a {nameof(SceneEventProgressStatus)}: {status}");
                isSceneLoading = false;
            }
            else
            {
                Debug.Log($"Scene {name} loaded!");
            }
        }
    }


    public bool alreadyConnected = false;
    public void Join(string uuid, string username, string authToken)
    {
        DevConsole.EnableConsole();
        NetworkManager.StartClient();
        Debug.Log("Start client");
        
        Debug.Log("Creating player...");
        StartCoroutine(CreatePlayer(uuid, username, authToken));
        
    }

    IEnumerator CreatePlayer(string uuid, string username, string authToken)
    {
        Debug.Log("Waiting network manager initialization...");
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        Debug.Log("Waiting local player initialization...");
        yield return new WaitUntil(() => NetworkManager.Singleton.LocalClient.PlayerObject != null);
        Debug.Log("Success creating of a player...");
        
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        CreateInitialPlayerData(player.gameObject, NetworkManager.Singleton.LocalClient.ClientId, player.NetworkObjectId, uuid, username, authToken);
        
        yield return new WaitUntil(() => player.GetComponent<NetworkObject>().IsSpawned);
        
        UIController.Singleton.StartUI();
        Camera.main.GetComponent<SmoothCameraTargetting>().StartTargetting();
    }

    private PlayerData CreateInitialPlayerData(GameObject player, ulong clientId, ulong networkId, string uuid, string username, string authToken)
    {
        var playerData = player.GetComponent<PlayerData>();

        playerData.uuid = uuid;
        playerData.authToken = authToken;
        playerData.ClientId = clientId;
        playerData.NetworkId = networkId;
        playerData.PlayerName = username;
        playerData.CurrentHealth = defaultData.currentHealth;
        playerData.MaxHealth = defaultData.maxHealth;
        playerData.HealthRegen = defaultData.healthRegen;
        playerData.MaxShield = defaultData.maxShield;
        playerData.CurrentShield = defaultData.currentShield;
        playerData.ShieldRegen = defaultData.shieldRegen;
        playerData.MaxMana = defaultData.maxMana;
        playerData.CurrentMana = defaultData.currentMana;
        playerData.ManaRegen = defaultData.manaRegen;
        playerData.MaxStamina = defaultData.maxStamina;
        playerData.CurrentStamina = defaultData.currentStamina;
        playerData.StaminaRegenRate = defaultData.staminaRegenRate;
        playerData.StaminaPerMove = defaultData.staminaPerMove;
        playerData.MovementSpeed = defaultData.movementSpeed;
        playerData.Armor = defaultData.armor;
        playerData.AttackDamage = defaultData.attackDamage;

        return playerData;
    }
}
