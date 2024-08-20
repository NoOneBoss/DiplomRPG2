using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "ItemPool", menuName = "Items/ItemPool")]
    public class ItemPool : ScriptableObject
    {
        public List<Item> items = new List<Item>();
    }
}