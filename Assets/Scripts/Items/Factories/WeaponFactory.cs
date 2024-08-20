using UnityEngine;

namespace Items.Specification.Factories
{
    [CreateAssetMenu(fileName = "WeaponFactory", menuName = "Items/Specifications/WeaponFactory")]
    public class WeaponFactory : ItemFactory
    {
        public ModifierFactory defaultDamage;

        public override Item getItem(byte amount)
        {
            return new Weapon(id, cost, displayName, description, stackSize, amount,
                ItemCategory.Weapon, modifiers, sprite, defaultDamage);
        }
    }
}