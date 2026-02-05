using System;
using System.Collections.Generic;
using ThirdPersonInventoryDemo.Inventory;

namespace ThirdPersonInventoryDemo.Items
{
    public class ItemDatabase
    {
        private readonly Dictionary<string, ItemData> _byId = new();

        public void Add(ItemData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Id))
                throw new ArgumentException("Invalid item data");

            _byId[data.Id] = data;
        }

        public bool TryGet(string id, out ItemData data) => _byId.TryGetValue(id, out data);
        public ItemData Get(string id)
        {
            if (!_byId.TryGetValue(id, out var data))
                throw new KeyNotFoundException($"Item id not found: {id}");
            return data;
        }
    }
}
