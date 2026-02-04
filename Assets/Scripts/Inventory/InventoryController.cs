using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameStateManager _gameState;
    [SerializeField] private InventoryView _view;

    private InventorySystem _inventorySystem;
    public InventorySystem InventorySystem => _inventorySystem;

    void OnEnable()
    {
        _playerInput.InventoryPressed += OnInventoryPressed;
    }

    void OnDisable()
    {
        _playerInput.InventoryPressed -= OnInventoryPressed;
    }

    private void Awake()
    {
        _inventorySystem = new InventorySystem(12);
    }

    private void OnGUI()
    {
        int slotWidth = 80;
        int slotHeight = 80;
        int padding = 10;
        int columns = 4;

        int startX = 20;
        int startY = 20;

        for (int i = 0; i < _inventorySystem.Slots.Length; i++)
        {
            int x = startX + (i % columns) * (slotWidth + padding);
            int y = startY + (i / columns) * (slotHeight + padding);

            GUI.Box(new Rect(x, y, slotWidth, slotHeight), "");

            var slot = _inventorySystem.Slots[i];

            if (!slot.IsEmpty)
            {
                GUI.Label(new Rect(x + 5, y + 5, slotWidth - 10, 20), slot.ItemData.Name);

                GUI.Label(new Rect(x + 5, y + 25, slotWidth - 10, 20), $"x{slot.Quantity}");
            }
        }
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

}
