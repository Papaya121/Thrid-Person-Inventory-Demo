using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Camera _camera;

    [Header("Settings")]
    [SerializeField] private float _range = 2f;
    [SerializeField] private LayerMask _itemMask;

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

    private void TryPickup()
    {
        if (_camera == null) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _range, _itemMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                print("INTERACT");
            }
        }
    }
}
