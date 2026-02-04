using System;
using System.Collections.Generic;
using UnityEngine;


public class PickupFactory : MonoBehaviour
{
    [Header("Spawn settings")]
    [SerializeField] private float _randomYaw = 180f;
    [SerializeField] private bool _applyColorToAllRenderers = true;

    [Header("References")]
    [SerializeField] private GameObject _pickupPrefab;

    private ItemDatabase _itemDatabase;

    public void Construct(ItemDatabase itemDatabase)
    {
        _itemDatabase = itemDatabase;
    }

    public WorldItem Spawn(string itemId, int quantity, Vector3 position)
    {
        if (_pickupPrefab == null)
        {
            Debug.LogError("[PickupFactory] Pickup prefab is not assigned.");
            return null;
        }

        if (_itemDatabase == null)
        {
            Debug.LogError("[PickupFactory] ItemDatabase is not constructed.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogError("[PickupFactory] itemId is null/empty.");
            return null;
        }

        if (!_itemDatabase.TryGet(itemId, out var data))
        {
            Debug.LogError($"[PickupFactory] Unknown item id: {itemId}");
            return null;
        }

        quantity = Mathf.Max(1, quantity);

        Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-_randomYaw, _randomYaw), 0f);
        GameObject go = Instantiate(_pickupPrefab, position, rotation);

        var pickup = go.GetComponent<WorldItem>();
        if (pickup == null)
            pickup = go.AddComponent<WorldItem>();

        pickup.Setup(itemId, quantity);

        ApplyColor(go, data.Color);

        return pickup;
    }

    private void ApplyColor(GameObject go, Color color)
    {
        if (_applyColorToAllRenderers)
        {
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
                SetRendererColor(r, color);
        }
        else
        {
            var r = go.GetComponentInChildren<Renderer>(true);
            if (r != null) SetRendererColor(r, color);
        }
    }

    private static void SetRendererColor(Renderer renderer, Color color)
    {
        if (renderer == null) return;
        if (renderer.material != null)
            renderer.material.color = color;
    }
}
