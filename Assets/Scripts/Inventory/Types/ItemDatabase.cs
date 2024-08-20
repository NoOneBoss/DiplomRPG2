using System.Collections.Generic;
using Items;
using Other;
using UnityEngine;

namespace Inventory.Types
{
    public static class ItemDatabase {
        static Dictionary<string, ItemFactory> itemFactoryDictionary;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize() {
            itemFactoryDictionary = new Dictionary<string, ItemFactory>();

            var itemDetails = Resources.LoadAll<ItemFactory>("Items/Item");
            foreach (var item in itemDetails) {
                itemFactoryDictionary.Add(item.id, item);
            }
            Debug.Log($"Loaded {itemFactoryDictionary.Count} items factories from database!");
        }

        public static ItemFactory GetFactoryById(string id) {
            try {
                return itemFactoryDictionary[id];
            } catch {
                Debug.LogError($"Cannot find item details with id {id}");
                return null;
            }
        }
    }
}