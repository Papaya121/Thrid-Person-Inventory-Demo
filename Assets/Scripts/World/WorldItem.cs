using UnityEngine;
using ThirdPersonInventoryDemo.Interfaces;

namespace ThirdPersonInventoryDemo.World
{
    public class WorldItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _itemId = "";
        [SerializeField, Min(1)] private int _quantity = 1;

        public string ItemId => _itemId;
        public int Quantity => _quantity;

        public void Setup(string itemId, int quantity)
        {
            _itemId = itemId;
            _quantity = quantity;
        }

        public void ApplyColor(Color color)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
                SetRendererColor(r, color);
        }

        public bool CanInteract() => true;

        private static void SetRendererColor(Renderer renderer, Color color)
        {
            if (renderer == null) return;
            if (renderer.material != null)
                renderer.material.color = color;
        }
    }
}
