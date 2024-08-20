using System;
using Items;
using Other;
using UnityEngine;

namespace Inventory.Types
{
    [Serializable]
    public class InventoryData {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public Item[] Items;
        public int Capacity;
    }
}