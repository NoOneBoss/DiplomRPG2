using System;
using UnityEngine;

namespace Configs
{
    [Serializable]
    [CreateAssetMenu(fileName = "DefaultPlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerDataObject : ScriptableObject
    {
        [Header("Health")]
        [SerializeField] public float maxHealth;
        [SerializeField] public float currentHealth;
        [SerializeField] public float healthRegen;
        
        [Header("Mana")]
        [SerializeField] public float maxMana;
        [SerializeField] public float currentMana;
        [SerializeField] public float manaRegen;
        
        [Header("Shield")]
        [SerializeField] public float maxShield;
        [SerializeField] public float currentShield;
        [SerializeField] public float shieldRegen;

        [Header("Stamina")]
        [SerializeField] public float maxStamina;
        [SerializeField] public float currentStamina;
        [SerializeField] public float staminaRegenRate;
        [SerializeField] public float staminaPerMove;
        
        [Header("Stats")]
        [SerializeField] public float movementSpeed;
        [SerializeField] public float armor;
        [SerializeField] public float attackDamage;
    }
}