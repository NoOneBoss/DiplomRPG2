using Items.ModifierSpecification;
using UnityEngine;
using Random = System.Random;

namespace Items.Specification.Factories
{
    [CreateAssetMenu(fileName = "ModifierFactory", menuName = "Items/ModifierFactory")]
    public class ModifierFactory : ScriptableObject
    {
        public ModifierType modifierType;
        
        public Sprite positiveIcon, negativeIcon;
        public string name;
        [TextArea] public string description;
        
        public Vector2 value;

        public Modifier getModifier()
        {
            Random random = new Random();
            switch (modifierType)
            {
                case ModifierType.MAX_HEALTH:
                    return new MaxHealthModifier(modifierType, random.Next((int)value.x, (int)value.y), name, description,
                        positiveIcon, negativeIcon);
            }
            
            return new MaxHealthModifier(modifierType, random.Next((int)value.x, (int)value.y), name, description,
                positiveIcon, negativeIcon);
        }
    }
}