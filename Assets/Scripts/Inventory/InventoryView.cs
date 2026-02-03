using System;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : MonoBehaviour
{
    public bool IsOpen => _isOpen;
    private bool _isOpen = false;

    public void SetVisible(bool opened)
    {
        _isOpen = opened;
    }
}
