using System.Collections.Generic;
using Configs;
using Items.ModifierSpecification;
using Items.Specification.Factories;
using Other;
using UnityEngine;

namespace Items.Specification
{
    public class Weapon : Item
    {
        public ModifierFactory defaultDamage;

        public Weapon(string id, short cost, string displayName, string description, byte stackSize,
            byte currentAmount, ItemCategory category, IEnumerable<ModifierFactory> modifiers, Sprite sprite,
            ModifierFactory defaultDamage)
            : base(id, cost, displayName, description, stackSize, currentAmount, category, modifiers, sprite)
        {
            this.defaultDamage = defaultDamage;
            
            this.modifiers.Add(this.defaultDamage.getModifier());
        }
    }
}