using UnityEngine;

public class InventoryBootstrap : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private PickupFactory _pickupFactory;
    // [SerializeField] private InventoryView _inventoryView;

    private void Awake()
    {
        var db = new ItemDatabase();

        // TODO: Load JSON сюда
        db.Add(new ItemData("apple", "Apple", Color.red, true, 9));

        _inventoryController.Construct(db);
        _pickupFactory.Construct(db);

        // _inventoryView.Construct(db, _inventoryController.InventorySystem);
    }

    private void Start()
    {
        _pickupFactory.Spawn("apple", 3, new Vector3(0, 0.5f, 0));
    }
}
