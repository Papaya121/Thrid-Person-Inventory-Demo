using System;
using UnityEngine;

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
}