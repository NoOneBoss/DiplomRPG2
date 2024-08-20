using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Items.Specification.Factories;
using UnityEngine;

namespace Items
{
    public abstract class ItemFactory : ScriptableObject
    {
        public string id;
        public short cost;
        public string displayName;
        public string description;

        public byte stackSize;
        [HideInInspector] public ItemCategory category;
        public List<ModifierFactory> modifiers;
        
        public Sprite sprite;

        public abstract Item getItem(byte amount);
    }

    
    [Serializable]
    public abstract class Item
    {
        public string id;
        public short cost;
        public string displayName;
        public string description;

        public byte currentAmount;
        public byte stackSize;
        public ItemCategory category;
        public List<Modifier> modifiers;
        
        public Sprite sprite;
        
        public static Action ItemEquipEvent;
        public static Action ItemUnequipEvent;

        protected Item(string id, short cost, string displayName, string description, byte stackSize, byte currentAmount, ItemCategory category, IEnumerable<ModifierFactory> modifiers, Sprite sprite)
        {
            this.id = id;
            this.cost = cost;
            this.displayName = displayName;
            this.description = description;
            this.currentAmount = currentAmount;
            this.stackSize = stackSize;
            this.category = category;
            this.modifiers = modifiers.Select(x => x.getModifier()).ToList();
            this.sprite = sprite;
        }

        public virtual void Equip(PlayerData playerData)
        {
            foreach (Modifier modifier in modifiers)
            {
                modifier.ApplyModifier(playerData);
            }

            ItemEquipEvent.Invoke();
        }

        public virtual void Unequip(PlayerData playerData)
        {
            foreach (Modifier modifier in modifiers)
            {
                modifier.UnapplyModifier(playerData);
            }
            
            ItemUnequipEvent.Invoke();
        }
    }
    
    public enum ItemCategory
    {
        Weapon,
        Armor,
        Consumable,
        Accessory
    }
}