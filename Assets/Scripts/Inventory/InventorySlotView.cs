using UnityEngine;
using UnityEngine.UIElements;

namespace ThirdPersonInventoryDemo.Inventory
{
    public sealed class InventorySlotView
    {
        public readonly VisualElement Root;
        private readonly VisualElement _inner;
        private readonly VisualElement _icon;
        private readonly Label _qty;
        private readonly Label _name;

        public int Index { get; }

        public InventorySlotView(int index)
        {
            Index = index;

            Root = new VisualElement();
            Root.style.position = Position.Relative;
            Root.AddToClassList("slot");

            _inner = new VisualElement();
            _inner.AddToClassList("slot-inner");
            _inner.style.position = Position.Absolute;
            _inner.style.left = 0;
            _inner.style.top = 0;
            _inner.style.right = 0;
            _inner.style.bottom = 0;

            _icon = new VisualElement();
            _icon.AddToClassList("slot-icon");
            _icon.style.position = Position.Absolute;
            _icon.style.left = 0;
            _icon.style.top = 0;
            _icon.style.right = 0;
            _icon.style.bottom = 0;

            _qty = new Label();
            _qty.AddToClassList("slot-qty");
            _qty.style.position = Position.Absolute;

            _name = new Label();
            _name.AddToClassList("slot-name");
            _name.pickingMode = PickingMode.Ignore;
            _name.style.position = Position.Absolute;
            _name.style.left = 0;
            _name.style.right = 0;
            _name.style.top = 0;
            _name.style.bottom = 0;

            Root.Add(_inner);
            Root.Add(_icon);
            Root.Add(_name);
            Root.Add(_qty);

            SetEmpty();
        }

        public void SetEmpty()
        {
            _icon.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
            _qty.text = "";
            _qty.style.display = DisplayStyle.None;
            _name.text = "";
            _name.style.display = DisplayStyle.None;
        }

        public void SetItem(Color color, int quantity, bool showQuantity, string name)
        {
            _icon.style.backgroundColor = new StyleColor(color);

            if (!string.IsNullOrEmpty(name))
            {
                _name.text = name;
                _name.style.display = DisplayStyle.Flex;
            }
            else
            {
                _name.text = "";
                _name.style.display = DisplayStyle.None;
            }

            if (showQuantity && quantity > 1)
            {
                _qty.text = quantity.ToString();
                _qty.style.display = DisplayStyle.Flex;
            }
            else
            {
                _qty.text = "";
                _qty.style.display = DisplayStyle.None;
            }
        }

        public void SetHover(bool on)
        {
            if (on) Root.AddToClassList("slot--hover");
            else Root.RemoveFromClassList("slot--hover");
        }

        public void SetDragging(bool on)
        {
            if (on) Root.AddToClassList("slot--dragging");
            else Root.RemoveFromClassList("slot--dragging");
        }
    }
}
