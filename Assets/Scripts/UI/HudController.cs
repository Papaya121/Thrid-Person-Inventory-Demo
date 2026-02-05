using UnityEngine;
using UnityEngine.UIElements;
using ThirdPersonInventoryDemo.Player.Interaction;

namespace ThirdPersonInventoryDemo.UI
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private UIDocument _doc;
        [SerializeField] private InteractionController _interaction;

        private VisualElement _crosshair;
        private VisualElement _pickupHint;

        private void Awake()
        {
            if (_doc == null || _interaction == null)
            {
                Debug.LogError("[HudController] Missing references.");
                enabled = false;
                return;
            }

            var root = _doc.rootVisualElement;
            _crosshair = root.Q<VisualElement>("Crosshair");
            _pickupHint = root.Q<VisualElement>("PickupHint");
        }

        private void OnEnable()
        {
            _interaction.OnAimAtInteractableChanged += SetInteractState;
        }

        private void OnDisable()
        {
            _interaction.OnAimAtInteractableChanged -= SetInteractState;
        }

        private void SetInteractState(bool canInteract)
        {
            if (canInteract) _crosshair.AddToClassList("active");
            else _crosshair.RemoveFromClassList("active");

            if (_pickupHint == null) return;
            if (canInteract) _pickupHint.AddToClassList("visible");
            else _pickupHint.RemoveFromClassList("visible");
        }
    }
}
