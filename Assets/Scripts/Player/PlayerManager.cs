using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;


namespace Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Singleton;
        public NetworkVariable<NetworkSerializableArray<PlayerData>> players = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
        
        private void Start()
        {
            Singleton = this;
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback += OnClientDisconnectedCallback;
            }
            
            Debug.Log($"NetworkVariable is {players.Value} when spawned.");
            players.OnValueChanged += OnPlayerJoin;
        }
        
        private void OnPlayerJoin(NetworkSerializableArray<PlayerData> previous, NetworkSerializableArray<PlayerData> current)
        {
            Debug.Log($"Detected NetworkVariable Change: Previous: {previous.values.Length} | Current: {current.values.Length}");

            foreach (var currentValue in current.values)
            {
                UpdatePlayerTextOnClientRpc(currentValue.clientId, currentValue.playerName);
            }
        }

        [Rpc(SendTo.Server)]
        private void UpdatePlayerTextOnClientRpc(ulong clientId, string playerName)
        {
            Debug.Log("Changing playerNameTexts");
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
            {
                Debug.Log($"Try to change playerNameText for {clientId} to {playerName}");
                TargetUpdatePlayerTextRpc(networkClient.ClientId, playerName);
            }
        }

        [Rpc(SendTo.NotServer)]
        private void TargetUpdatePlayerTextRpc(ulong id, string playerName)
        {
            Debug.Log($"Change playerNameText for {id} to {playerName}");
            GameObject.FindGameObjectsWithTag("Player")
                .First(player => player.GetComponent<NetworkObject>().OwnerClientId == id).gameObject
                .GetComponentInChildren<TextMeshProUGUI>().text = playerName;
        }
        
        private void OnClientConnectedCallback(ulong obj)
        {
            Debug.Log($"Player{obj} connected to the server!");
        }
        
        private void OnClientDisconnectedCallback(ulong obj)
        {
            Debug.Log($"Player{obj} disconnected to the server!");
            UpdatePlayersDataRpc(obj);
        }
        
        [Rpc(SendTo.Server)]
        private void UpdatePlayersDataRpc(ulong id)
        {
            var currentPlayerDataArray = players.Value;
            var currentValues = new List<PlayerData>(currentPlayerDataArray.values.Where(x => x.clientId != id));

            players.Value = new NetworkSerializableArray<PlayerData>(currentValues.ToArray());
            Debug.Log($"Update players data (Count: {players.Value.values.Length})");
        }
    }

    [Serializable]
    public class NetworkSerializableArray<T> : INetworkSerializable where T : INetworkSerializable, new()
    {
        public T[] values = Array.Empty<T>();

        public NetworkSerializableArray(T[] values)
        {
            this.values = values;
        }

        public NetworkSerializableArray()
        {
        }

        public void NetworkSerialize<TSerializer>(BufferSerializer<TSerializer> serializer) where TSerializer : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteNetworkSerializable(values.ToArray());
            }
            else
            {
                serializer.GetFastBufferReader().ReadNetworkSerializable(out values);
            }
        }
    }
    

    [Serializable]
    [CreateAssetMenu]
    public class PlayerData : ScriptableObject, INetworkSerializable
    {
        [SerializeField] public ulong clientId;
        [SerializeField] public ulong networkId;
        [SerializeField] public string playerName;

        [Header("Health")]
        [SerializeField] public float maxHealth;
        [SerializeField] public float currentHealth;

        [Header("Movement")]
        [SerializeField] public float defaultMovementSpeed;
        [SerializeField] public float maxStamina;
        [SerializeField] public float currentStamina;
        [SerializeField] public float staminaRegenRate;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref networkId);
            serializer.SerializeValue(ref playerName);
            
            serializer.SerializeValue(ref maxHealth);
            serializer.SerializeValue(ref currentHealth);
            
            serializer.SerializeValue(ref defaultMovementSpeed);
            serializer.SerializeValue(ref maxStamina);
            serializer.SerializeValue(ref currentStamina);
            serializer.SerializeValue(ref staminaRegenRate);
        }
        
        
    }
    
    public class PlayerDataHandler : MonoBehaviour
    {
        public PlayerData playerData;
    }
    
}