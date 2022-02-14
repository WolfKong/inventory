using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ScrollPool<T> where T : Component
{
    private ScrollRect _scrollRect;
    private RectTransform _content;
    private Dictionary<int, T> _items;
    private ObjectPool<T> _pool;
    private Action<T, int> _initializeItem;

    private int _itemCount;
    private float _cellHeight;
    private float _viewPortHeight;

    /// <summary>
    /// Set sizes and creates pool.
    /// </summary>
    /// <param name="scrollRect">ScrollRect where pool will be created.</param>
    /// <param name="prefab">Prefab to generate items.</param>
    /// <param name="initializeItem">Action ran when making item visible.</param>
    public void Initialize(ScrollRect scrollRect, T prefab, Action<T, int> initializeItem)
    {
        _scrollRect = scrollRect;
        _content = _scrollRect.content;
        _initializeItem = initializeItem;

        _scrollRect.onValueChanged.AddListener(OnValueChanged);

        _cellHeight = prefab.GetComponent<RectTransform>().rect.height;
        _viewPortHeight = _scrollRect.viewport.rect.height;

        var visibleCells = Mathf.CeilToInt(_viewPortHeight / _cellHeight) + 1;

        _items = new Dictionary<int, T>();

        _pool = new ObjectPool<T>();
        _pool.PopulatePool(prefab, _content, visibleCells);
    }

    /// <summary>
    /// Set item count and content size.
    /// </summary>
    /// <param name="itemCount">Total item count.</param>
    public void SetItemCount(int itemCount)
    {
        _itemCount = itemCount;

        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemCount * _cellHeight);
    }

    /// <summary>
    /// Places all items near given index.
    /// </summary>
    /// <param name="index">Index of selected cell.</param>
    /// <param name="visiblePreviousCells">How many previous cells should be visible.</param>
    public void PlaceItems(int index, int visiblePreviousCells = 2)
    {
        var targetPosition = (index - visiblePreviousCells) * _cellHeight;

        // Avoid trying to scroll lower or higher than allowed
        var position = Mathf.Min(targetPosition, _content.rect.height - _viewPortHeight);
        position = Mathf.Max(position, 0);

        _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, position);

        _scrollRect.StopMovement();

        UpdateVisibleItems();
    }

    /// <summary>
    /// Returns all objects to pool
    /// </summary>
    public void ClearDisplay()
    {
        foreach (var item in _items)
            _pool.ReturnObject(item.Value);

        _items.Clear();
    }

    /// <summary>
    /// Returns scroll topmost visible item.
    /// </summary>
    /// <returns>Topmost visible item</returns>
    public T GetTopItem()
    {
        var topIndex = CellIndexForPosition(_content.anchoredPosition.y);
        return _items[topIndex];
    }

    private void PlaceObjectAt(int index)
    {
        var item = _pool.GetObject();

        _initializeItem(item, index);

        var rectTransform = item.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = new Vector2(0, -index * _cellHeight);

        _items[index] = item;
    }

    private void OnValueChanged(Vector2 _)
    {
        UpdateVisibleItems();
    }

    /// <summary>
    /// Removes no longer visible items and adds newly visible ones.
    /// </summary>
    private void UpdateVisibleItems()
    {
        var newTopIndex = CellIndexForPosition(_content.anchoredPosition.y);
        var newBottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);

        // Don't add items beyond content limits
        if (newTopIndex < 0)
            newTopIndex = 0;

        if (newBottomIndex >= _itemCount)
            newBottomIndex = _itemCount - 1;

        // Remove no longer visible items
        var indexesToRemove = new List<int>();
        foreach (var item in _items)
        {
            var index = item.Key;
            if (index > newBottomIndex || index < newTopIndex)
            {
                _pool.ReturnObject(item.Value);
                indexesToRemove.Add(index);
            }
        }

        foreach (var index in indexesToRemove)
            _items.Remove(index);

        // Add newly visible items
        for (int i = newTopIndex; i <= newBottomIndex; i++)
        {
            if (!_items.ContainsKey(i))
            {
                PlaceObjectAt(i);
            }
        }
    }

    /// <summary>
    /// Returns cell index for given Y position.
    /// </summary>
    /// <param name="position">Y position</param>
    /// <returns>Cell index</returns>
    private int CellIndexForPosition(float position) => Mathf.FloorToInt(position / _cellHeight);
}
