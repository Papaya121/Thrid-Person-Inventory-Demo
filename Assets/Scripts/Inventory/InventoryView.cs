using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThirdPersonInventoryDemo.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument _doc;

        private VisualElement _root;
        private VisualElement _grid;
        private VisualElement _dragGhost;
        private Label _dragGhostName;

        private readonly List<InventorySlotView> _slotViews = new();

        private InventorySystem _inv;
        private Action[] _slotHandlers;

        private bool _dragging;
        private int _dragFromIndex = -1;
        private Vector2 _pointerPos;

        public bool IsOpen { get; private set; }

        public event Action<int> RequestDropToWorld;

        private void Awake()
        {
            if (_doc == null) _doc = GetComponent<UIDocument>();
            _root = _doc.rootVisualElement;


            _grid = _root.Q<VisualElement>("grid");
            _dragGhost = _root.Q<VisualElement>("dragGhost");
            _dragGhostName = new Label();
            _dragGhostName.AddToClassList("drag-ghost-name");
            _dragGhostName.pickingMode = PickingMode.Ignore;
            _dragGhost.Add(_dragGhostName);

            _root.RegisterCallback<PointerMoveEvent>(e =>
            {
                if (!_dragging) return;
                _pointerPos = e.position;
                UpdateGhostPosition();
            });

            _root.RegisterCallback<PointerUpEvent>(e =>
            {
                if (!_dragging || e.button != 0) return;

                if (!IsPointerOverAnySlot(e.position))
                {
                    RequestDropToWorld?.Invoke(_dragFromIndex);
                    CancelDrag();
                }
            });

            SetVisible(false);
        }

        public void Bind(InventorySystem inventory)
        {
            UnsubscribeSlots();

            _inv = inventory;

            BuildGrid();
            SubscribeSlots();
            RefreshAll();
        }

        private void BuildGrid()
        {
            _grid.Clear();
            _slotViews.Clear();

            for (int i = 0; i < _inv.Slots.Length; i++)
            {
                var sv = new InventorySlotView(i);
                _slotViews.Add(sv);
                _grid.Add(sv.Root);

                RegisterSlotEvents(sv);
            }
        }

        private void SubscribeSlots()
        {
            if (_inv == null) return;
            _slotHandlers = new Action[_inv.Slots.Length];

            for (int i = 0; i < _inv.Slots.Length; i++)
            {
                int idx = i;
                Action handler = () => RefreshSlot(idx);
                _slotHandlers[i] = handler;
                _inv.Slots[i].SlotChanged += handler;
            }
        }

        private void UnsubscribeSlots()
        {
            if (_inv == null || _slotHandlers == null) return;

            int count = Mathf.Min(_inv.Slots.Length, _slotHandlers.Length);
            for (int i = 0; i < count; i++)
            {
                if (_slotHandlers[i] != null)
                    _inv.Slots[i].SlotChanged -= _slotHandlers[i];
            }

            _slotHandlers = null;
        }

        public void SetVisible(bool opened)
        {
            IsOpen = opened;
            _root.Q<VisualElement>("inventoryRoot").style.display = opened ? DisplayStyle.Flex : DisplayStyle.None;

            if (!opened)
                CancelDrag();
        }

        private void RefreshAll()
        {
            for (int i = 0; i < _inv.Slots.Length; i++)
                RefreshSlot(i);
        }

        private void RefreshSlot(int index)
        {
            var slot = _inv.Slots[index];
            var view = _slotViews[index];

            if (slot.IsEmpty)
            {
                view.SetEmpty();
                return;
            }

            bool showQty = slot.ItemData.IsStackable;
            view.SetItem(slot.ItemData.Color, slot.Quantity, showQty, slot.ItemData.Name);
        }

        private void RegisterSlotEvents(InventorySlotView sv)
        {
            sv.Root.RegisterCallback<PointerDownEvent>(e =>
            {
                if (!IsOpen) return;

                if (e.button == 0)
                {
                    TryBeginDrag(sv.Index, e.position);
                    e.StopPropagation();
                }

                if (e.button == 1)
                {
                    if (!_inv.Slots[sv.Index].IsEmpty)
                    {
                        RequestDropToWorld?.Invoke(sv.Index);
                    }
                    e.StopPropagation();
                }
            });

            sv.Root.RegisterCallback<PointerEnterEvent>(_ => sv.SetHover(true));
            sv.Root.RegisterCallback<PointerLeaveEvent>(_ => sv.SetHover(false));

            sv.Root.RegisterCallback<PointerUpEvent>(e =>
            {
                if (!_dragging || e.button != 0) return;

                int toIndex = sv.Index;
                PerformDrop(toIndex);
                e.StopPropagation();
            });
        }

        private bool IsPointerOverAnySlot(Vector2 pos)
        {
            foreach (var sv in _slotViews)
            {
                if (sv.Root.worldBound.Contains(pos))
                    return true;
            }
            return false;
        }

        private void TryBeginDrag(int fromIndex, Vector2 pointerPos)
        {
            var fromSlot = _inv.Slots[fromIndex];
            if (fromSlot.IsEmpty) return;

            _dragging = true;
            _dragFromIndex = fromIndex;
            _pointerPos = pointerPos;

            _dragGhost.style.display = DisplayStyle.Flex;
            _dragGhost.style.backgroundColor = new StyleColor(fromSlot.ItemData.Color);
            _dragGhostName.text = fromSlot.ItemData.Name;
            _dragGhostName.style.display = DisplayStyle.Flex;
            _slotViews[_dragFromIndex].SetDragging(true);
            UpdateGhostPosition();
        }

        private void UpdateGhostPosition()
        {
            float w = _dragGhost.resolvedStyle.width > 0 ? _dragGhost.resolvedStyle.width : 90f;
            float h = _dragGhost.resolvedStyle.height > 0 ? _dragGhost.resolvedStyle.height : 90f;

            _dragGhost.style.left = _pointerPos.x - w * 0.5f;
            _dragGhost.style.top = _pointerPos.y - h * 0.5f;
        }

        private void PerformDrop(int toIndex)
        {
            if (_dragFromIndex < 0 || toIndex < 0) { CancelDrag(); return; }
            if (_dragFromIndex == toIndex) { CancelDrag(); return; }

            var from = _inv.Slots[_dragFromIndex];
            var to = _inv.Slots[toIndex];

            if (to.IsEmpty)
            {
                _inv.SwapItem(_dragFromIndex, toIndex);
                CancelDrag();
                return;
            }

            if (!from.IsEmpty && !to.IsEmpty &&
                from.ItemData.Id == to.ItemData.Id &&
                from.ItemData.IsStackable)
            {
                int max = to.MaxStack;
                int canAdd = max - to.Quantity;
                if (canAdd > 0)
                {
                    int move = Mathf.Min(canAdd, from.Quantity);

                    to.TryAddQuantity(move);

                    from.TryRemoveQuantity(move);
                }

                CancelDrag();
                return;
            }

            _inv.SwapItem(_dragFromIndex, toIndex);
            CancelDrag();
        }

        private void CancelDrag()
        {
            _dragging = false;
            if (_dragFromIndex >= 0 && _dragFromIndex < _slotViews.Count)
                _slotViews[_dragFromIndex].SetDragging(false);
            _dragFromIndex = -1;
            _dragGhostName.text = "";
            _dragGhostName.style.display = DisplayStyle.None;
            _dragGhost.style.display = DisplayStyle.None;
        }
    }
}
