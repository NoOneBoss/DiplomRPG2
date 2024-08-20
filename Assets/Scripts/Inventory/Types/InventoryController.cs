using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Other;
using UnityEngine;

namespace Inventory.Types
{
    public class ViewModel {
        public readonly int Capacity;

        public ViewModel(InventoryModel model, int capacity) {
            Capacity = capacity;
        }
    }
    
    public class InventoryController { 
        readonly InventoryView view;
        readonly InventoryModel model;
        readonly int capacity;

        InventoryController(InventoryView view, InventoryModel model, int capacity) {
            this.view = view;
            this.model = model;
            this.capacity = capacity;

            view.StartCoroutine(Initialize());
        }
        
        public void Bind(InventoryData data) => model.Bind(data);

        IEnumerator Initialize() {
            yield return view.InitializeView(new ViewModel(model, capacity));
            
            view.OnDrop += HandleDrop;
            model.OnModelChanged += HandleModelChanged;

            RefreshView();
        }

        void HandleDrop(Slot originalSlot, Slot closestSlot) {
            if (originalSlot.Index == closestSlot.Index || closestSlot.ItemId.Equals(string.Empty)) {
                model.Swap(originalSlot.Index, closestSlot.Index);
                return;
            }
            
            var sourceItemId = model.Get(originalSlot.Index).id;
            var targetItemId = model.Get(closestSlot.Index).id;
                        
            if (sourceItemId.Equals(targetItemId) && model.Get(closestSlot.Index).stackSize > 1) { 
                model.Combine(originalSlot.Index, closestSlot.Index);
            } else {
                model.Swap(originalSlot.Index, closestSlot.Index);
            }
        }

        void HandleModelChanged(IList<Item> items) => RefreshView();
        
        void RefreshView() {
            for (int i = 0; i < capacity; i++) {
                var item = model.Get(i);
                if (item == null || item.id.Equals(string.Empty)) {
                    view.Slots[i].Set(string.Empty, null);
                } else {
                    view.Slots[i].Set(item.id, item.sprite, item.currentAmount);
                }
            }
        }
        
        #region Builder
        
        public class Builder {
            InventoryView view;
            IEnumerable<Item> items;
            int capacity = 30;
            
            public Builder(InventoryView view) {
                this.view = view;
            }

            public Builder WithStartingItems(IEnumerable<Item> items) {
                this.items = items;
                return this;
            }

            public Builder WithCapacity(int capacity) {
                this.capacity = capacity;
                return this;
            }

            public InventoryController Build() {
                InventoryModel model = items != null 
                    ? new InventoryModel(items, capacity) 
                    : new InventoryModel(Array.Empty<Item>(), capacity);

                return new InventoryController(view, model, capacity);
            }
        }
        
        #endregion Builder
    }
}