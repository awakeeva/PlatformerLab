using PixelCrew.Model.Data.Properties;
using System;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    public class QuickInvetoryModel : MonoBehaviour
    {
        private readonly PlayerData _data;

        public InventoryItemData[] Inventory { get; private set; }

        public readonly IntProperty SelectedIndex = new IntProperty();

        public QuickInvetoryModel(PlayerData data)
        {
            this._data = data;

            Inventory = _data.Inventory.GetAll();
            _data.Inventory.onChanged += OnChanged;
        }

        private void OnChanged(string id, int value)
        {
            Inventory = _data.Inventory.GetAll();
            SelectedIndex.Value = Mathf.Clamp(SelectedIndex.Value, 0, Inventory.Length - 1);
        }
    }
}

