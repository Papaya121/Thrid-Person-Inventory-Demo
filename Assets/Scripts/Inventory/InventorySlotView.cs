using UnityEngine;
using UnityEngine.UIElements;

public sealed class InventorySlotView
{
    public readonly VisualElement Root;
    private readonly VisualElement _icon;
    private readonly Label _qty;

    public int Index { get; }

    public InventorySlotView(int index)
    {
        Index = index;

        Root = new VisualElement();
        Root.AddToClassList("slot");

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

        Root.Add(_icon);
        Root.Add(_qty);

        SetEmpty();
    }

    public void SetEmpty()
    {
        _icon.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
        _qty.text = "";
        _qty.style.display = DisplayStyle.None;
    }

    public void SetItem(Color color, int quantity, bool showQuantity)
    {
        _icon.style.backgroundColor = new StyleColor(color);

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
}
