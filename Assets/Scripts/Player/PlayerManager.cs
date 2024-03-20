using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player
{

    namespace Player
    {
        public class PlayerManager : NetworkBehaviour
        {
            public static PlayerManager Singleton;
            public static Dictionary<GameObject, PlayerData> players = new Dictionary<GameObject, PlayerData>();
            public List<PlayerDataHandler> datas = new List<PlayerDataHandler>();
            
            public PlayerData playerDataTemplate;

            private void Start()
            {
                if (IsServer || IsHost || ServerIsHost)
                {
                    Singleton = this;
                    Debug.Log("Start Player Manager");
                }

                GameObject.FindWithTag("NetworkManager").GetComponent<JoinGame>().Join();
            }

            public static void spawnPlayer(GameObject obj, PlayerData data)
            {
                Debug.Log("spawnPlayer");
                PlayerDataHandler handler = obj.AddComponent<PlayerDataHandler>();
                handler.InitializePlayerData(data);

                players[obj] = data;
                Singleton.datas.Add(handler);
            }
        }
    }

    
    [Serializable]
    [CreateAssetMenu]
    public class PlayerData : ScriptableObject
    {
        public TextMeshProUGUI playerNameLabel;
        
        
        public string playerName;

        [Header("Health")]
        public float maxHealth;
        public float currentHealth;

        [Header("Movement")]
        public float defaultMovementSpeed;
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenRate;
    }
    

    namespace Player
    {
        public class PlayerDataHandler : MonoBehaviour
        {
            public PlayerData playerData;

            public void InitializePlayerData(PlayerData data)
            {
                playerData = data;
            }
        }
    }


}