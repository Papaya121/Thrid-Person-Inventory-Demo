using System;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _range = 2f;
    [SerializeField] private LayerMask _itemMask;

    [Header("References")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private InventoryController _inventoryController;

    public event Action<bool> OnAimAtInteractableChanged;

    private bool _isAiming;

    private void OnEnable()
    {
        if (_input == null) return;
        _input.InteractPressed += TryPickup;
    }

    private void OnDisable()
    {
        if (_input == null) return;
        _input.InteractPressed -= TryPickup;
    }

    private void Update()
    {
        bool aimingNow = CheckInteractable();

        if (aimingNow != _isAiming)
        {
            _isAiming = aimingNow;
            OnAimAtInteractableChanged?.Invoke(_isAiming);
        }
    }

    private bool CheckInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _itemMask);

        if (colliders.Length == 0) return false;

        Collider closestCollider = GetClosestInteractable(colliders);

        return closestCollider != null;
    }

    private Collider GetClosestInteractable(Collider[] colliders)
    {
        Collider closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestCollider = collider;
                    closestDistance = distance;
                }
            }
        }

        return closestCollider;
    }

    private void TryPickup()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _itemMask);

        if (colliders.Length == 0) return;

        Collider closestCollider = GetClosestInteractable(colliders);

        if (closestCollider != null)
        {
            IInteractable interactable = closestCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                var worldItem = interactable as WorldItem;
                if (worldItem == null) return;

                bool success = false;
                if (_inventoryController.ItemDatabase.TryGet(worldItem.ItemId, out var itemData))
                    success = _inventoryController.InventorySystem.TryAddItem(itemData, worldItem.Quantity);

                if (success)
                    Destroy(worldItem.gameObject);
            }
        }
    }
}
