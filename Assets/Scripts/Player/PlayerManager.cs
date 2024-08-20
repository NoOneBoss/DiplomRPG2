using Configs;
using Other;
using Unity.Netcode;
using UnityEngine;


namespace Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Singleton;
        public NetworkObject prefab;

        private void Start()
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        public PlayerData getLocalPlayerData()
        {
            return NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<PlayerData>();
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback += OnClientDisconnectedCallback;
            }
        }
        
        
        private void OnClientConnectedCallback(ulong obj)
        {
            StartCoroutine(Schedulers.ExecuteAfterTime(1f, () =>
            {
                Debug.Log($"{NetworkManager.ConnectedClients[obj].PlayerObject.GetComponent<PlayerData>().playerName.Value} connected to the server!");
            }));
            /*NetworkManager.SpawnManager.InstantiateAndSpawn(
                prefab, NetworkManager.ServerClientId, true,
                false, false, new Vector3(-37.1f, 0f, 0f), Quaternion.identity);*/
        }
        
        private void OnClientDisconnectedCallback(ulong obj)
        {
            Debug.Log($"{NetworkManager.ConnectedClients[obj].PlayerObject.GetComponent<PlayerData>().playerName.Value} disconnected to the server!");
        }
    }
}