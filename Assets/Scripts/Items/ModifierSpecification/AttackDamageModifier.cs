using System;
using Configs;
using Player;
using UnityEngine;

namespace Items.ModifierSpecification
{
    public class AttackDamageModifier : Modifier
    {
        public AttackDamageModifier(ModifierType modifierType, float value, string name, string description, Sprite positiveIcon, Sprite negativeIcon)
            : base(modifierType, value, name, description, positiveIcon, negativeIcon) { }

        public override void ApplyModifier(PlayerData playerData)
        {
            playerData.AttackDamage += value;
            if (playerData.AttackDamage <= 0) playerData.AttackDamage = 1;
        }

        public override void UnapplyModifier(PlayerData playerData)
        {
            playerData.AttackDamage -= value;
            if (playerData.AttackDamage <= 0) playerData.AttackDamage = 1;
        }
    }
}