using System;
using Configs;
using UnityEngine;

namespace Items
{
    [Serializable]
    public abstract class Modifier
    {
        public ModifierType modifierType;
        public float value;
        
        public string name;
        public string description;
        public Sprite icon;
        
        public abstract void ApplyModifier(PlayerData playerData);
        public abstract void UnapplyModifier(PlayerData playerData);

        protected Modifier(ModifierType modifierType, float value, string name, string description, Sprite positiveIcon, Sprite negativeIcon)
        {
            this.modifierType = modifierType;
            this.value = value;
            this.name = name;
            if (value > 0)
            {
                this.description = description.Replace("{action}", "увеличивает").Replace("{value}", $"{Math.Abs(value)}");
                this.icon = positiveIcon;
            }
            else
            {
                this.description = description.Replace("{action}", "уменьшает").Replace("{value}", $"{Math.Abs(value)}");
                this.icon = negativeIcon;
            }
            
        }
    }
}