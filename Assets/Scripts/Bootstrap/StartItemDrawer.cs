using UnityEngine;
using ThirdPersonInventoryDemo.Items;
using ThirdPersonInventoryDemo.World;

namespace ThirdPersonInventoryDemo.Bootstrap
{
    public class StartItemDrawer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemDatabaseProvider _dbProvider;

        void Start()
        {
            var items = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);

            foreach (var item in items)
            {
                if (_dbProvider.Database.TryGet(item.ItemId, out var data))
                    item.ApplyColor(data.Color);
            }
        }
    }
}
