using System.Collections;
using Other;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventory.Types
{
    public class InventoryView : StorageView
    {
        [SerializeField] private string title = "Инвентарь";
        public override IEnumerator InitializeView(ViewModel viewModel) {
            Slots = new Slot[viewModel.Capacity];
            root = document.rootVisualElement;
            root.Clear(); 

            root.styleSheets.Add(styleSheet);
 
            container = root.CreateChild("container");
            
            var inventory = container.CreateChild("inventory").WithManipulator(new PanelDragManipulator());
            inventory.CreateChild("inventoryFrame");
            inventory.CreateChild("inventoryHeader").Add(new Label(title));

            var slotsContainer = inventory.CreateChild("slotsContainer");
            for (int i = 0; i < viewModel.Capacity; i++) {
                var slot = slotsContainer.CreateChild<Slot>("slot");
                Slots[i] = slot;
            }
            
            ghostIcon = container.CreateChild("ghostIcon");
            
            yield return null; 
        }
    }
}