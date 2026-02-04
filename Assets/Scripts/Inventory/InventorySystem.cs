using System;
using UnityEngine;

public class InventorySystem
{
    private SlotData[] _slots;
    public SlotData[] Slots => _slots;

    public InventorySystem(int slotsCount)
    {
        _slots = new SlotData[slotsCount];
        for (int i = 0; i < slotsCount; i++)
        {
            _slots[i] = new SlotData();
        }
    }

    public bool TryAddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity < 1) return false;

        foreach (var slot in _slots)
        {
            if (slot.IsContainId(itemData.Id))
            {
                int canAdd = slot.MaxStack - slot.Quantity;
                int addQuantity = Mathf.Clamp(quantity, 0, canAdd);

                if (addQuantity > 0)
                {
                    bool success = slot.TryAddQuantity(addQuantity);
                    quantity -= addQuantity;

                    if (quantity <= 0) return true;
                }
            }
        }

        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                int addQuantity = Mathf.Clamp(quantity, 1, itemData.MaxStack);
                slot.TrySetItem(itemData, addQuantity);
                quantity -= addQuantity;

                if (quantity <= 0) return true;
            }
        }

        return quantity <= 0;
    }

    public bool TryRemoveItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity < 1) return false;

        for (int i = _slots.Length - 1; i >= 0; i--)
        {
            var slot = _slots[i];

            if (slot.IsContainId(itemData.Id))
            {
                int canRemove = slot.Quantity;
                int removeQuantity = Mathf.Clamp(quantity, 0, canRemove);

                if (removeQuantity > 0)
                {
                    bool success = slot.TryRemoveQuantity(removeQuantity);
                    quantity -= removeQuantity;

                    if (quantity <= 0) return true;
                }
            }
        }

        return quantity <= 0;
    }

    public bool IsContainItem(ItemData itemData, int quantity = 1)
    {
        int totalQuantity = 0;

        foreach (var slot in _slots)
        {
            if (slot.IsContainId(itemData.Id))
            {
                totalQuantity += slot.Quantity;

                if (totalQuantity >= quantity)
                    return true;
            }
        }

        return totalQuantity >= quantity;
    }

    public bool SwapItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _slots.Length) return false;
        if (toIndex < 0 || toIndex >= _slots.Length) return false;
        if (fromIndex == toIndex) return false;

        var fromSlot = _slots[fromIndex];
        var toSlot = _slots[toIndex];

        ItemData tempItem = fromSlot.ItemData;
        int tempQty = fromSlot.Quantity;

        fromSlot.TrySetItem(toSlot.ItemData, toSlot.Quantity);
        toSlot.TrySetItem(tempItem, tempQty);

        return true;
    }

    public SlotData GetSlot(int index) => _slots[index];

    public bool TrySetSlot(int index, ItemData item, int qty)
    {
        if (index < 0 || index >= _slots.Length) return false;
        return _slots[index].TrySetItem(item, qty);
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= _slots.Length) return;
        _slots[index].RemoveItem();
    }
}

public class ItemData
{
    private string _id;
    private string _name;
    private Color _color;
    private bool _isStackable;
    private int _maxStack;

    public string Id => _id;
    public string Name => _name;
    public Color Color => _color;
    public bool IsStackable => _isStackable;
    public int MaxStack => _maxStack;

    public ItemData(string id, string name, Color color, bool isStackable = false, int maxStack = 99)
    {
        _id = id;
        _name = name;
        _color = color;
        _isStackable = isStackable;
        _maxStack = maxStack;
    }
}
public class SlotData
{
    private ItemData _itemData;
    private int _quantity;

    public ItemData ItemData => _itemData;
    public int Quantity => _quantity;
    public int MaxStack => _itemData != null ? _itemData.MaxStack : 1;

    public bool IsEmpty => _itemData == null;


    public Action SlotChanged;

    public SlotData()
    {
        _itemData = null;
        _quantity = 0;

        SlotChanged?.Invoke();
    }

    public SlotData(ItemData itemData, int quantity = 1)
    {
        TrySetItem(itemData, quantity);
    }

    public bool TrySetItem(ItemData itemData, int quantity = 1)
    {
        if (itemData != null && quantity > itemData.MaxStack) return false;

        _itemData = itemData;
        _quantity = itemData != null ? quantity : 0;

        SlotChanged?.Invoke();
        return true;
    }

    public void RemoveItem() => TrySetItem(null, 0);

    public bool IsContainId(string id)
    {
        return _itemData?.Id == id;
    }

    public bool TryAddQuantity(int quantity)
    {
        if (_itemData == null || quantity < 1) return false;

        var maxStack = _itemData.MaxStack;
        if (_quantity + quantity > maxStack)
            return false;

        _quantity += quantity;

        SlotChanged?.Invoke();
        return true;
    }

    public bool TryRemoveQuantity(int quantity)
    {
        if (_itemData == null || quantity < 1) return false;

        if (_quantity - quantity < 0)
            return false;

        _quantity -= quantity;

        if (_quantity <= 0)
            _itemData = null;

        SlotChanged?.Invoke();
        return true;
    }
}