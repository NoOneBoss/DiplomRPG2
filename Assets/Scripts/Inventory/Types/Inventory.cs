using System.Collections.Generic;
using Items;
using Other;
using UnityEngine;

namespace Inventory.Types
{
    public class Inventory : MonoBehaviour {
        [SerializeField] InventoryView view;
        [SerializeField] int capacity = 30;
        [SerializeField] List<Item> items = new List<Item>();

        InventoryController controller;

        void Awake() {
            controller = new InventoryController.Builder(view)
                .WithStartingItems(items)
                .WithCapacity(capacity)
                .Build();
        }
         
        public void Bind(InventoryData data) {
            controller.Bind(data);
        }
    }
}