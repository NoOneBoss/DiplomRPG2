using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Singleton;
        
        public PlayerData playerData;
        private void Start()
        {
            Singleton = this;
            
            playerData = ScriptableObject.CreateInstance<PlayerData>();
            playerData.playerName = "nooneboss";
            playerData.currentHealth = 100f;
            playerData.maxHealth = 100f;
            playerData.defaultMovementSpeed = 0.2f;
            playerData.maxStamina = 100f;
            playerData.currentStamina = 100f;
            playerData.staminaRegenRate = 10f;
            
            
            UIController.Singleton.StartUI();
        }
    }
    
    
    [Serializable][CreateAssetMenu]
    public class PlayerData : ScriptableObject
    {
        public string playerName;

        [Header("Health")]
        public float maxHealth;
        public float currentHealth;
        
        [Header("Movement")]
        public float defaultMovementSpeed;
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenRate;
        
        
        static PlayerData()
        {
            var group = new ConverterGroup("Health Converter");
            group.AddConverter((ref float health) => $"{health}/{PlayerManager.Singleton.playerData.maxHealth}");
            ConverterGroups.RegisterConverterGroup(group);
        }
    }
}