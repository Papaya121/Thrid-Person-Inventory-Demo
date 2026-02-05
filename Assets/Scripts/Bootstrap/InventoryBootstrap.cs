using System.IO;
using UnityEngine;
using ThirdPersonInventoryDemo.Inventory;
using ThirdPersonInventoryDemo.Items;
using ThirdPersonInventoryDemo.World;

namespace ThirdPersonInventoryDemo.Bootstrap
{
    public class InventoryBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private PickupFactory _pickupFactory;
        [SerializeField] private ItemDatabaseProvider _itemDatabaseProvider;

        private void Awake()
        {
            var db = new ItemDatabase();

            var jsonPath = Path.Combine(Application.streamingAssetsPath, "items.json");
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                ItemDatabaseJsonLoader.FillDatabase(db, json);
            }
            else
            {
                Debug.LogError($"[InventoryBootstrap] Item database JSON not found: {jsonPath}");
            }

            _inventoryController.Construct(db);
            _pickupFactory.Construct(db);

            _itemDatabaseProvider.Set(db);
        }
    }
}
