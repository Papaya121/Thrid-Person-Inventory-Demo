using System.Collections.Generic;
using System.IO;
using ThirdPersonInventoryDemo.Items;
using ThirdPersonInventoryDemo.World;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldItem)), CanEditMultipleObjects]
public class WorldItemEditor : Editor
{
    private const string DefaultFileName = "items.json";

    private SerializedProperty _itemIdProp;
    private SerializedProperty _quantityProp;

    private readonly List<string> _itemIds = new();
    private readonly List<string> _itemLabels = new();
    private string _loadError;

    private void OnEnable()
    {
        _itemIdProp = serializedObject.FindProperty("_itemId");
        _quantityProp = serializedObject.FindProperty("_quantity");
        LoadItems();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHeader();
        EditorGUILayout.Space(4);

        DrawItemPopup();
        EditorGUILayout.PropertyField(_itemIdProp, new GUIContent("Item Id"));
        EditorGUILayout.PropertyField(_quantityProp, new GUIContent("Quantity"));

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Reload Items", GUILayout.Width(120)))
                LoadItems();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.TextField(GetItemsFilePath());
        }

        if (!string.IsNullOrWhiteSpace(_loadError))
            EditorGUILayout.HelpBox(_loadError, MessageType.Warning);
    }

    private void DrawItemPopup()
    {
        if (_itemIds.Count == 0)
        {
            EditorGUILayout.HelpBox("No items found in JSON. Use Item Database Editor or edit the JSON file.", MessageType.Info);
            return;
        }

        var currentId = _itemIdProp.stringValue ?? string.Empty;
        var popupIds = _itemIds;
        var popupLabels = _itemLabels;

        int index = popupIds.IndexOf(currentId);
        if (index < 0)
        {
            var tempIds = new List<string>();
            var tempLabels = new List<string>();
            if (!string.IsNullOrWhiteSpace(currentId))
            {
                tempIds.Add(currentId);
                tempLabels.Add($"Missing: {currentId}");
            }

            tempIds.AddRange(_itemIds);
            tempLabels.AddRange(_itemLabels);
            popupIds = tempIds;
            popupLabels = tempLabels;
            index = Mathf.Clamp(index, 0, popupIds.Count - 1);
        }

        int newIndex = EditorGUILayout.Popup("Item", Mathf.Max(0, index), popupLabels.ToArray());
        if (newIndex >= 0 && newIndex < popupIds.Count)
            _itemIdProp.stringValue = popupIds[newIndex];
    }

    private void LoadItems()
    {
        _itemIds.Clear();
        _itemLabels.Clear();
        _loadError = null;

        var path = GetItemsFilePath();
        if (!File.Exists(path))
        {
            _loadError = "Items JSON not found. Create it via Item Database Editor.";
            return;
        }

        var json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json))
        {
            _loadError = "Items JSON is empty.";
            return;
        }

        var parsed = JsonUtility.FromJson<ItemDatabaseJson>(json);
        if (parsed?.items == null || parsed.items.Length == 0)
        {
            _loadError = "Items JSON has no items.";
            return;
        }

        foreach (var item in parsed.items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.id))
                continue;

            _itemIds.Add(item.id);
            var label = string.IsNullOrWhiteSpace(item.name) ? item.id : $"{item.name} ({item.id})";
            _itemLabels.Add(label);
        }

        if (_itemIds.Count == 0)
            _loadError = "Items JSON has no valid item ids.";
    }

    private static string GetItemsFilePath()
    {
        return Path.Combine(Application.streamingAssetsPath, DefaultFileName);
    }
}
