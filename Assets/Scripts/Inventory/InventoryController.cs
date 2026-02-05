using UnityEngine;
using ThirdPersonInventoryDemo.Core;
using ThirdPersonInventoryDemo.Player.Input;
using ThirdPersonInventoryDemo.World;
using ThirdPersonInventoryDemo.Items;

namespace ThirdPersonInventoryDemo.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameStateManager _gameState;
        [SerializeField] private InventoryView _view;
        [SerializeField] private PickupFactory _pickupFactory;

        private InventorySystem _inventorySystem;
        public InventorySystem InventorySystem => _inventorySystem;

        private ItemDatabase _itemDatabase;
        public ItemDatabase ItemDatabase => _itemDatabase;

        public void Construct(ItemDatabase db)
        {
            _itemDatabase = db;
        }

        void OnEnable()
        {
            _playerInput.InventoryPressed += OnInventoryPressed;
        }

        void OnDisable()
        {
            _playerInput.InventoryPressed -= OnInventoryPressed;
            _view.RequestDropToWorld -= OnRequestDropToWorld;
        }

        private void Awake()
        {
            if (_playerInput == null || _gameState == null || _view == null)
            {
                Debug.LogError("[InventoryController] Missing references.");
                enabled = false;
                return;
            }

            _inventorySystem = new InventorySystem(12);
            _view.RequestDropToWorld += OnRequestDropToWorld;
        }

        private void Start()
        {
            _view.Bind(_inventorySystem);
        }

        private void OnInventoryPressed()
        {
            ToggleInventory();
        }

        public void ToggleInventory()
        {
            bool opened = !_view.IsOpen;

            _view.SetVisible(opened);

            _gameState.SetMode(opened
                ? GameMode.Inventory
                : GameMode.Gameplay);
        }

        private void OnRequestDropToWorld(int slotIndex)
        {
            var slot = _inventorySystem.Slots[slotIndex];
            if (slot.IsEmpty) return;

            Debug.Log($"Drop to world: {slot.ItemData.Name} x{slot.Quantity}");

            if (_pickupFactory && _playerInput)
            {
                Vector3 dropPos =
                    _playerInput.transform.position +
                    _playerInput.transform.forward * 1.5f +
                    Vector3.up * 0.5f;

                _pickupFactory.Spawn(slot.ItemData.Id, slot.Quantity, dropPos);

            }

            slot.RemoveItem();
        }

    }
}
