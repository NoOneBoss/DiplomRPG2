using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DavidFDev.DevConsole;
using Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGame : NetworkBehaviour
{
    private bool isSceneLoading = false;

    private void Start()
    {
        Join();
    }
    
    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsServer && !isSceneLoading)
        {
            isSceneLoading = true;
            var status = NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load scene with a {nameof(SceneEventProgressStatus)}: {status}");
                isSceneLoading = false;
            }
            else
            {
                Debug.Log("Scene loaded!");
            }
        }
    }

    private void Join()
    {
        if (Application.isBatchMode)
        {
            NetworkManager.StartServer();
        }
        
        if(NetworkManager.IsServer) return;
        
        NetworkManager.StartClient();
        Debug.Log("Start client");
        
        Debug.Log("Creating player...");
        StartCoroutine(CreatePlayer());
        DevConsole.EnableConsole();
    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        yield return new WaitUntil(() => NetworkManager.Singleton.LocalClient.PlayerObject != null);

        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        var playerData = CreateInitialPlayerData(NetworkManager.Singleton.LocalClient.ClientId, player.NetworkObjectId);

        UpdatePlayersDataRpc(playerData);

        PlayerDataHandler dataHandler = player.gameObject.AddComponent<PlayerDataHandler>();
        dataHandler.playerData = playerData;

        var playerNameLabel = player.GetComponentInChildren<TextMeshProUGUI>();
        if (playerNameLabel.GetComponentInParent<NetworkObject>().IsOwner) playerNameLabel.enabled = false;

        UIController.Singleton.StartUI();
        Camera.main.GetComponent<SmoothCameraTargetting>().StartTargetting();

        yield return new WaitUntil(() => player.GetComponent<NetworkObject>().IsSpawned);
    }

    private PlayerData CreateInitialPlayerData(ulong clientId, ulong networkId)
    {
        var playerData = ScriptableObject.CreateInstance<PlayerData>();
        playerData.clientId = clientId;
        playerData.networkId = networkId;
        playerData.playerName = $"player{networkId}";
        playerData.currentHealth = 100f;
        playerData.maxHealth = 100f;
        playerData.defaultMovementSpeed = 0.5f;
        playerData.maxStamina = 100f;
        playerData.currentStamina = 100f;
        playerData.staminaRegenRate = 10f;
        return playerData;
    }


    [Rpc(SendTo.Server)]
    private void UpdatePlayersDataRpc(PlayerData playerData)
    {
        var currentPlayerDataArray = PlayerManager.Singleton.players.Value;
        var currentValues = new List<PlayerData>(currentPlayerDataArray.values) { playerData };

        PlayerManager.Singleton.players.Value = new NetworkSerializableArray<PlayerData>(currentValues.ToArray());
        Debug.Log($"Update players data (Count: {PlayerManager.Singleton.players.Value.values.Length})");
    }
}
