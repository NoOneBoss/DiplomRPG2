using System;
using Configs;
using Player;
using UnityEngine;

namespace Items.ModifierSpecification
{
    public class MaxHealthModifier : Modifier
    {
        public MaxHealthModifier(ModifierType modifierType, float value, string name, string description, Sprite positiveIcon, Sprite negativeIcon)
            : base(modifierType, value, name, description, positiveIcon, negativeIcon) { }

        public override void ApplyModifier(PlayerData playerData)
        {
            playerData.MaxHealth += value;
            if (playerData.CurrentHealth > playerData.MaxHealth) playerData.CurrentHealth = playerData.MaxHealth;
            if (playerData.CurrentHealth <= 0) playerData.CurrentHealth = 1;
        }

        public override void UnapplyModifier(PlayerData playerData)
        {
            playerData.MaxHealth -= value;
            if (playerData.CurrentHealth <= 0) playerData.CurrentHealth = 1;
        }
    }
}