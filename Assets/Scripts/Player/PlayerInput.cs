using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Actions (Input System)")]
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _look;
    [SerializeField] private InputActionReference _interact;
    [SerializeField] private InputActionReference _inventory;

    public event Action<Vector2> MoveChanged;
    public event Action<Vector2> LookChanged;
    public event Action InteractPressed;
    public event Action InventoryPressed;

    public Vector2 MoveValue { get; private set; }
    public Vector2 LookValue { get; private set; }

    private void OnEnable()
    {
        if (_move != null)
        {
            _move.action.Enable();
            _move.action.performed += OnMove;
            _move.action.canceled += OnMove;
        }

        if (_look != null)
        {
            _look.action.Enable();
            _look.action.performed += OnLook;
            _look.action.canceled += OnLook;
        }

        if (_interact != null)
        {
            _interact.action.Enable();
            _interact.action.performed += OnInteract;
        }

        if (_inventory != null)
        {
            _inventory.action.Enable();
            _inventory.action.performed += OnInventory;
        }

    }

    private void OnDisable()
    {
        if (_move != null)
        {
            _move.action.performed -= OnMove;
            _move.action.canceled -= OnMove;
            _move.action.Disable();
        }

        if (_look != null)
        {
            _look.action.performed -= OnLook;
            _look.action.canceled -= OnLook;
            _look.action.Disable();
        }

        if (_interact != null)
        {
            _interact.action.performed -= OnInteract;
            _interact.action.Disable();
        }


        if (_inventory != null)
        {
            _inventory.action.performed -= OnInventory;
            _inventory.action.Disable();
        }
    }


    private void OnInventory(InputAction.CallbackContext ctx)
    {
        InventoryPressed?.Invoke();
    }


    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveValue = ctx.ReadValue<Vector2>();
        MoveChanged?.Invoke(MoveValue);
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        LookValue = ctx.ReadValue<Vector2>();
        LookChanged?.Invoke(LookValue);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        InteractPressed?.Invoke();
    }
}
