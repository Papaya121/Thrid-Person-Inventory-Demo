using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameStateManager _gameState;
    [SerializeField] private InventoryView _view;

    void OnEnable()
    {
        _playerInput.InventoryPressed += OnInventoryPressed;
    }

    void OnDisable()
    {
        _playerInput.InventoryPressed -= OnInventoryPressed;
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
