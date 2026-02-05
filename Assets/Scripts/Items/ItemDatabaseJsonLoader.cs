using System;
using UnityEngine;
using ThirdPersonInventoryDemo.Inventory;

namespace ThirdPersonInventoryDemo.Items
{
    [Serializable]
    public class ItemDatabaseJson
    {
        public ItemJson[] items;
    }

    [Serializable]
    public class ItemJson
    {
        public string id;
        public string name;
        public string color;
        public bool stackable;
        public int maxStack = 99;
    }

    public static class ItemDatabaseJsonLoader
    {
        public static void FillDatabase(ItemDatabase db, string json)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[ItemDatabaseJsonLoader] JSON is empty.");
                return;
            }

            var parsed = JsonUtility.FromJson<ItemDatabaseJson>(json);
            if (parsed == null || parsed.items == null || parsed.items.Length == 0)
            {
                Debug.LogError("[ItemDatabaseJsonLoader] JSON has no items.");
                return;
            }

            foreach (var item in parsed.items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.id))
                    continue;

                var color = ParseColor(item.color);
                int maxStack = item.stackable ? Mathf.Max(1, item.maxStack) : 1;

                db.Add(new ItemData(item.id, item.name, color, item.stackable, maxStack));
            }
        }

        private static Color ParseColor(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Color.white;

            var v = value.Trim();
            if (!v.StartsWith("#", StringComparison.Ordinal))
                v = "#" + v;

            if (ColorUtility.TryParseHtmlString(v, out var color))
                return color;

            Debug.LogWarning($"[ItemDatabaseJsonLoader] Invalid color '{value}', fallback to white.");
            return Color.white;
        }
    }
}
