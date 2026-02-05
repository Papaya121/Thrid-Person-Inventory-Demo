using UnityEngine;

namespace ThirdPersonInventoryDemo.Items
{
    public class ItemDatabaseProvider : MonoBehaviour
    {
        public ItemDatabase Database { get; private set; }

        public void Set(ItemDatabase db)
        {
            Database = db;
        }
    }
}
