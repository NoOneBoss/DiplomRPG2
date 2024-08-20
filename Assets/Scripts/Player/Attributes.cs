using System;
using Unity.Netcode;

namespace Player
{
    [Serializable]
    public class HealthAttribute : Attribute
    {
        public float maxHealth;
        public float currentHealth;
        public float healthRegen;
        public long lastModifiedTime;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out maxHealth);
                reader.ReadValueSafe(out currentHealth);
                reader.ReadValueSafe(out healthRegen);
                reader.ReadValueSafe(out lastModifiedTime);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(maxHealth);
                writer.WriteValueSafe(currentHealth);
                writer.WriteValueSafe(healthRegen);
                writer.WriteValueSafe(lastModifiedTime);
            }
        }
    }
    
    [Serializable]
    public class ShieldAttribute : Attribute
    {
        public float maxShield;
        public float currentShield;
        public float shieldRegen;
        public long lastModifiedTime;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out maxShield);
                reader.ReadValueSafe(out currentShield);
                reader.ReadValueSafe(out shieldRegen);
                reader.ReadValueSafe(out lastModifiedTime);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(maxShield);
                writer.WriteValueSafe(currentShield);
                writer.WriteValueSafe(shieldRegen);
                writer.WriteValueSafe(lastModifiedTime);
            }
        }
    }
    
    [Serializable]
    public class ManaAttribute : Attribute
    {
        public float maxMana;
        public float currentMana;
        public float manaRegen;
        public long lastModifiedTime;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out maxMana);
                reader.ReadValueSafe(out currentMana);
                reader.ReadValueSafe(out manaRegen);
                reader.ReadValueSafe(out lastModifiedTime);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(maxMana);
                writer.WriteValueSafe(currentMana);
                writer.WriteValueSafe(manaRegen);
                writer.WriteValueSafe(lastModifiedTime);
            }
        }
    }
    
    [Serializable]
    public class StaminaAttribute : Attribute
    {
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenRate;
        public float staminaPerMove;
        public long lastModifiedTime;

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out maxStamina);
                reader.ReadValueSafe(out currentStamina);
                reader.ReadValueSafe(out staminaRegenRate);
                reader.ReadValueSafe(out staminaPerMove);
                reader.ReadValueSafe(out lastModifiedTime);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(maxStamina);
                writer.WriteValueSafe(currentStamina);
                writer.WriteValueSafe(staminaRegenRate);
                writer.WriteValueSafe(staminaPerMove);
                writer.WriteValueSafe(lastModifiedTime);
            }
        }
    }
    
    
    public abstract class Attribute : INetworkSerializable
    {
        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            throw new NotImplementedException();
        }
    }
}
