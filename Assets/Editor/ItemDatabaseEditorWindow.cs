using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThirdPersonInventoryDemo.Items;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ItemDatabaseEditorWindow : EditorWindow
{
    private const string DefaultFileName = "items.json";

    private readonly List<ItemJson> _items = new();
    private Vector2 _scroll;
    private ReorderableList _list;
    private readonly List<bool> _expanded = new();
    private bool _isDirty;

    private string _newId = "";
    private string _newName = "";
    private Color _newColor = Color.white;
    private bool _newStackable = true;
    private int _newMaxStack = 1;

    [MenuItem("Tools/Item Database Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<ItemDatabaseEditorWindow>();
        window.titleContent = new GUIContent("Item Database");
        window.minSize = new Vector2(520, 420);
        window.Show();
    }

    private void OnEnable()
    {
        LoadFromFile();
        SetupList();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        DrawHeader();
        EditorGUILayout.Space(8);

        DrawItemList();
        EditorGUILayout.Space(10);
        DrawAddSection();
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Items File", GUILayout.Width(70));
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(GetItemsFilePath());
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Reload", GUILayout.Width(100)))
                LoadFromFile();

            if (GUILayout.Button("Save", GUILayout.Width(100)))
                SaveToFile();
        }

        if (_isDirty)
            EditorGUILayout.HelpBox("There are unsaved changes.", MessageType.Warning);
    }

    private void DrawItemList()
    {
        EnsureExpandedSize();

        if (_items.Count == 0)
            EditorGUILayout.HelpBox("No items found in JSON.", MessageType.Info);

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
        _list?.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }

    private void DrawAddSection()
    {
        EditorGUILayout.LabelField("Add New Item", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            _newId = EditorGUILayout.TextField("Id", _newId);
            _newName = EditorGUILayout.TextField("Name", _newName);
            _newColor = EditorGUILayout.ColorField("Color", _newColor);
            _newStackable = EditorGUILayout.Toggle("Stackable", _newStackable);

            using (new EditorGUI.DisabledScope(!_newStackable))
            {
                _newMaxStack = EditorGUILayout.IntField("Max Stack", _newMaxStack);
            }

            if (!_newStackable)
                _newMaxStack = 1;

            if (GUILayout.Button("Add Item"))
                TryAddItem();
        }
    }

    private void TryAddItem()
    {
        var id = (_newId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(id))
        {
            EditorUtility.DisplayDialog("Add Item", "Id is required.", "OK");
            return;
        }

        if (_items.Any(x => x != null && string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase)))
        {
            EditorUtility.DisplayDialog("Add Item", "An item with this Id already exists.", "OK");
            return;
        }

        AddItemInternal(new ItemJson
        {
            id = id,
            name = string.IsNullOrWhiteSpace(_newName) ? id : _newName.Trim(),
            color = ToHex(_newColor),
            stackable = _newStackable,
            maxStack = _newStackable ? Mathf.Max(1, _newMaxStack) : 1
        });

        MarkDirty();
        ResetNewItemFields();
    }

    private void ResetNewItemFields()
    {
        _newId = string.Empty;
        _newName = string.Empty;
        _newColor = Color.white;
        _newStackable = true;
        _newMaxStack = 1;
    }

    private void LoadFromFile()
    {
        _items.Clear();
        _expanded.Clear();

        var path = GetItemsFilePath();
        if (!File.Exists(path))
        {
            SaveEmptyDatabase(path);
            return;
        }

        var json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json))
        {
            SaveEmptyDatabase(path);
            return;
        }

        var parsed = JsonUtility.FromJson<ItemDatabaseJson>(json);
        if (parsed?.items == null)
            return;

        _items.AddRange(parsed.items);
        EnsureExpandedSize();
        _isDirty = false;
    }

    private void SaveToFile()
    {
        var path = GetItemsFilePath();
        var data = new ItemDatabaseJson
        {
            items = _items.Where(x => x != null).ToArray()
        };

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
        _isDirty = false;
    }

    private void SaveEmptyDatabase(string path)
    {
        var data = new ItemDatabaseJson { items = Array.Empty<ItemJson>() };
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }

    private static string GetItemsFilePath()
    {
        return Path.Combine(Application.streamingAssetsPath, DefaultFileName);
    }

    private static Color ParseColor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Color.white;

        var v = value.Trim();
        if (!v.StartsWith("#", StringComparison.Ordinal))
            v = "#" + v;

        if (ColorUtility.TryParseHtmlString(v, out var color))
            return color;

        return Color.white;
    }

    private static string ToHex(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }

    private void SetupList()
    {
        _list = new ReorderableList(_items, typeof(ItemJson), true, true, true, true);
        _list.drawHeaderCallback = rect => GUI.Label(rect, "Items");
        _list.drawElementCallback = DrawListElement;
        _list.elementHeightCallback = GetElementHeight;
        _list.onAddCallback = _ => AddItemInternal(CreateDefaultItem());
        _list.onRemoveCallback = OnRemoveItem;
        EnsureExpandedSize();
    }

    private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index < 0 || index >= _items.Count)
            return;

        var item = _items[index];
        if (item == null)
            return;

        float line = EditorGUIUtility.singleLineHeight;
        float pad = 4f;
        rect.y += pad;
        rect.height = line;

        var headerRect = new Rect(rect.x, rect.y, rect.width, line);
        var foldoutRect = new Rect(headerRect.x, headerRect.y, headerRect.width - 60f, line);
        var colorRect = new Rect(headerRect.xMax - 54f, headerRect.y, 54f, line);

        var title = string.IsNullOrWhiteSpace(item.name) ? item.id : $"{item.name} ({item.id})";
        _expanded[index] = EditorGUI.Foldout(foldoutRect, _expanded[index], title, true);

        var previewColor = ParseColor(item.color);
        using (new EditorGUI.DisabledScope(true))
            EditorGUI.ColorField(colorRect, GUIContent.none, previewColor, true, false, false);

        if (!_expanded[index])
            return;

        rect.y += line + pad;
        EditorGUI.BeginChangeCheck();

        item.id = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, line), "Id", item.id ?? string.Empty).Trim();
        rect.y += line + pad;

        item.name = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, line), "Name", item.name ?? string.Empty);
        rect.y += line + pad;

        var color = ParseColor(item.color);
        color = EditorGUI.ColorField(new Rect(rect.x, rect.y, rect.width, line), "Color", color);
        item.color = ToHex(color);
        rect.y += line + pad;

        item.stackable = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, line), "Stackable", item.stackable);
        rect.y += line + pad;

        using (new EditorGUI.DisabledScope(!item.stackable))
        {
            int maxStack = EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width, line), "Max Stack", item.maxStack);
            item.maxStack = item.stackable ? Mathf.Max(1, maxStack) : 1;
        }

        if (!item.stackable)
            item.maxStack = 1;

        if (EditorGUI.EndChangeCheck())
            MarkDirty();
    }

    private float GetElementHeight(int index)
    {
        if (index < 0 || index >= _expanded.Count)
            return EditorGUIUtility.singleLineHeight + 8f;

        float line = EditorGUIUtility.singleLineHeight;
        float pad = 4f;
        if (!_expanded[index])
            return line + pad * 2f;

        int lines = 6;
        return (line + pad) * (lines + 1);
    }

    private void OnRemoveItem(ReorderableList list)
    {
        if (list.index < 0 || list.index >= _items.Count)
            return;

        var item = _items[list.index];
        var label = item == null ? "this item" : $"{item.name} ({item.id})";
        if (!EditorUtility.DisplayDialog("Remove Item", $"Remove {label}?", "Remove", "Cancel"))
            return;

        _items.RemoveAt(list.index);
        if (list.index < _expanded.Count)
            _expanded.RemoveAt(list.index);
        MarkDirty();
    }

    private void EnsureExpandedSize()
    {
        while (_expanded.Count < _items.Count)
            _expanded.Add(false);
        while (_expanded.Count > _items.Count)
            _expanded.RemoveAt(_expanded.Count - 1);
    }

    private ItemJson CreateDefaultItem()
    {
        var id = "new_item";
        int suffix = 1;
        while (_items.Any(x => x != null && string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase)))
        {
            id = $"new_item_{suffix}";
            suffix++;
        }

        return new ItemJson
        {
            id = id,
            name = "New Item",
            color = ToHex(Color.white),
            stackable = true,
            maxStack = 1
        };
    }

    private void AddItemInternal(ItemJson item)
    {
        _items.Add(item);
        _expanded.Add(true);
        MarkDirty();
    }

    private void MarkDirty()
    {
        _isDirty = true;
        Repaint();
    }
}
