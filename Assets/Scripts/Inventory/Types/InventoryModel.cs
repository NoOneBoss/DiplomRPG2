using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Inventory.Types
{
    public class InventoryModel { 
        ObservableArray<Item> Items { get; }
        InventoryData inventoryData = new InventoryData();
        readonly int capacity;

        public event Action<Item[]> OnModelChanged {
            add => Items.AnyValueChanged += value;
            remove => Items.AnyValueChanged -= value;
        }
        
        public InventoryModel(IEnumerable<Item> items, int capacity) {
            this.capacity = capacity;
            Items = new ObservableArray<Item>(capacity);
            foreach (var item in items) {
                Items.TryAdd(item);
            }
        }

        public void Bind(InventoryData data) {
            inventoryData = data;
            inventoryData.Capacity = capacity;
            
            bool isNew = inventoryData.Items == null || inventoryData.Items.Length == 0;

            if (isNew) {
                inventoryData.Items = new Item[capacity];
            }

            if (isNew && Items.Count != 0) {
                for (var i = 0; i < capacity; i++) {
                    if (Items[i] == null) continue;
                    inventoryData.Items[i] = Items[i];
                }
            }
            
            Items.items = inventoryData.Items;
        }
        
        public Item Get(int index) => Items[index];
        public void Clear() => Items.Clear();
        public bool Add(Item item) => Items.TryAdd(item);
        public bool Remove(Item item) => Items.TryRemove(item);
        public void Swap(int source, int target) => Items.Swap(source, target);
        public int Combine(int source, int target) {
            var total = Items[source].currentAmount + Items[target].currentAmount;
            Items[target].currentAmount = (byte) total;
            Remove(Items[source]);
            return total;
        }
    }
}