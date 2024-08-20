using System;
using Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class PlayerData : NetworkBehaviour
    {
        public string uuid;
        
        public NetworkVariable<ulong> clientId = new(writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<ulong> networkId = new(writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<FixedString32Bytes> playerName = new(writePerm: NetworkVariableWritePermission.Owner);
        public string authToken;
        
        //Attributes
        [HideInInspector] public NetworkVariable<HealthAttribute> healthAttribute = new(writePerm: NetworkVariableWritePermission.Owner);
        [HideInInspector] public NetworkVariable<ShieldAttribute> shieldAttribute = new(writePerm: NetworkVariableWritePermission.Owner);
        [HideInInspector] public NetworkVariable<ManaAttribute> manaAttribute = new(writePerm: NetworkVariableWritePermission.Owner);
        [HideInInspector] public NetworkVariable<StaminaAttribute> staminaAttribute = new(writePerm: NetworkVariableWritePermission.Owner);

        //Stats
        public NetworkVariable<float> movementSpeed = new(writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> armor = new(writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> attackDamage = new(writePerm: NetworkVariableWritePermission.Owner);

        
        public string playerNameUI;
        public float maxHealthUI;
        public float currentHealthUI;
        
        public float maxShieldUI;
        public float currentShieldUI;
        
        public float maxManaUI;
        public float currentManaUI;
        
        public float maxStaminaUI;
        public float currentStaminaUI;
        
        public float movementSpeedUI;
        public float armorUI;
        public float attackDamageUI;
        public int armorPercentageUI;
        
        public ulong ClientId
        {
            get => clientId.Value;
            set => clientId.Value = value;
        }
        
        public ulong NetworkId
        {
            get => networkId.Value;
            set => networkId.Value = value;
        }
        
        public string PlayerName
        {
            get => playerName.Value.Value;
            set
            {
                playerName.Value = value;
                playerNameUI = value;
            }
        }
        
        public float MaxHealth
        {
            get => healthAttribute.Value.maxHealth;
            set
            {
                healthAttribute.Value.maxHealth = value;
                maxHealthUI = value;
            }
        }

        public float CurrentHealth
        {
            get => healthAttribute.Value.currentHealth;
            set
            { 
                if(healthAttribute.Value.currentHealth > value) healthAttribute.Value.lastModifiedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                healthAttribute.Value.currentHealth = value;
                currentHealthUI = value;
            }
        }

        public float HealthRegen
        {
            get => healthAttribute.Value.healthRegen;
            set => healthAttribute.Value.healthRegen = value;
        }
        
        public float MaxShield
        {
            get => shieldAttribute.Value.maxShield;
            set
            {
                shieldAttribute.Value.maxShield = value;
                maxShieldUI = value;
            }
        }

        public float CurrentShield
        {
            get => shieldAttribute.Value.currentShield;
            set
            { 
                if(shieldAttribute.Value.currentShield > value) shieldAttribute.Value.lastModifiedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                shieldAttribute.Value.currentShield = value;
                currentShieldUI = value;
            }
        }

        public float ShieldRegen
        {
            get => shieldAttribute.Value.shieldRegen;
            set => shieldAttribute.Value.shieldRegen = value;
        }

        public float MaxMana
        {
            get => manaAttribute.Value.maxMana;
            set
            { 
                manaAttribute.Value.maxMana = value;
                maxManaUI = value;
            }
        }

        public float CurrentMana
        {
            get => manaAttribute.Value.currentMana;
            set
            { 
                if(manaAttribute.Value.currentMana > value) manaAttribute.Value.lastModifiedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                manaAttribute.Value.currentMana = value;
                currentManaUI = value;
            }
        }

        public float ManaRegen
        {
            get => manaAttribute.Value.manaRegen;
            set => manaAttribute.Value.manaRegen = value;
        }

        public float MovementSpeed
        {
            get => movementSpeed.Value;
            set
            {
                movementSpeed.Value = value;
                movementSpeedUI = value;
            }
        }

        public float MaxStamina
        {
            get => staminaAttribute.Value.maxStamina;
            set
            { 
                staminaAttribute.Value.maxStamina = value;
                maxStaminaUI = value;
            }
        }

        public float CurrentStamina
        {
            get => staminaAttribute.Value.currentStamina;
            set
            { 
                if(staminaAttribute.Value.currentStamina > value) staminaAttribute.Value.lastModifiedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                staminaAttribute.Value.currentStamina = value;
                currentStaminaUI = value;
            }
        }

        public float StaminaRegenRate
        {
            get => staminaAttribute.Value.staminaRegenRate;
            set => staminaAttribute.Value.staminaRegenRate = value;
        }

        public float StaminaPerMove
        {
            get => staminaAttribute.Value.staminaPerMove;
            set => staminaAttribute.Value.staminaPerMove = value;
        }

        public float Armor
        {
            get => armor.Value;
            set
            {
                armor.Value = value;
                armorUI = value;
                armorPercentageUI = (int) (100 * ((0.06 * value) / (1 + 0.06 * value)));
            }
        }
        
        public float AttackDamage
        {
            get => attackDamage.Value;
            set
            {
                attackDamage.Value = value;
                attackDamageUI = value;
            }
        }

        public bool Equals(PlayerData other)
        {
            return uuid == other.uuid;
        }
    }
}