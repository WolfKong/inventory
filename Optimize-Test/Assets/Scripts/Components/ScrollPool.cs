using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollPool<T> where T : Component
{
    private ScrollRect _scrollRect;
    private RectTransform _content;
    private T[] _items;
    private ObjectPool<T> _pool;
    private Action<T, int> _initializeItem;

    private int _itemCount;
    private int _topIndex;
    private int _bottomIndex;

    private float _cellHeight;
    private float _viewPortHeight;

    /// <summary>
    /// Set sizes and creates pool.
    /// </summary>
    /// <param name="prefab">Prefab to generate items.</param>
    /// <param name="initializeItem">Action that set's up item data.</param>
    /// <param name="itemCount">Total item count.</param>
    public void Initialize(ScrollRect scrollRect, T prefab, Action<T, int> initializeItem)
    {
        _initializeItem = initializeItem;
        _scrollRect = scrollRect;
        _content = _scrollRect.content;

        _scrollRect.onValueChanged.AddListener(OnValueChanged);

        _cellHeight = prefab.GetComponent<RectTransform>().rect.height;
        _viewPortHeight = _scrollRect.viewport.rect.height;

        _topIndex = CellIndexForPosition(_content.anchoredPosition.y);
        _bottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);

        _pool = new ObjectPool<T>();
        _pool.PopulatePool(prefab, _content, _bottomIndex - _topIndex + 2);
    }

    /// <summary>
    /// Set item count and content size.
    /// </summary>
    /// <param name="itemCount">Total item count.</param>
    public void SetItemCount(int itemCount)
    {
        _itemCount = itemCount;

        _items = new T[_itemCount];

        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemCount * _cellHeight);
    }

    /// <summary>
    /// Places all objects near given index.
    /// </summary>
    /// <param name="index">Index of selected cell.</param>
    public void PlaceItems(int index)
    {
        var contentSize = _content.sizeDelta.y;
        // Targeting index - 2 to show some previous items
        var targetPosition = (index - 2) * _cellHeight;

        // Avoid trying to scroll lower or higher than allowed
        var position = Mathf.Min(targetPosition, contentSize - _viewPortHeight);
        position = Mathf.Max(targetPosition, 0);

        _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, position);

        _scrollRect.StopMovement();

        _topIndex = CellIndexForPosition(_content.anchoredPosition.y);
        _bottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);

        for (int i = _topIndex; i <= _bottomIndex; i++)
            PlaceObjectAt(i);
    }

    /// <summary>
    /// Returns all objects to pool
    /// </summary>
    public void ClearDisplay()
    {
        if (_items == null)
            return;

        foreach (var item in _items)
        {
            if (item != null)
            {
                _pool.ReturnObject(item);
            }
        }
    }

    /// <summary>
    /// Returns scroll topmost visible item.
    /// </summary>
    /// <returns>Topmost visible item</returns>
    public T GetTopItem()
    {
        return _items[_topIndex];
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

        // Don't change items on scroll limits
        if (newTopIndex < 0 || newBottomIndex >= _itemCount)
            return;

        // Remove no longer visible items
        while (newTopIndex > _topIndex)
        {
            _pool.ReturnObject(_items[_topIndex]);
            _items[_topIndex] = null;
            _topIndex++;
        }

        while (newBottomIndex < _bottomIndex)
        {
            _pool.ReturnObject(_items[_bottomIndex]);
            _items[_bottomIndex] = null;
            _bottomIndex--;
        }

        // Add newly visible items
        while (newTopIndex < _topIndex)
        {
            _topIndex--;
            PlaceObjectAt(_topIndex);
        }

        while (newBottomIndex > _bottomIndex)
        {
            _bottomIndex++;
            PlaceObjectAt(_bottomIndex);
        }
    }

    /// <summary>
    /// Returns cell index for given Y position.
    /// </summary>
    /// <param name="position">Y position</param>
    /// <returns>Cell index</returns>
    private int CellIndexForPosition(float position) => Mathf.FloorToInt(position / _cellHeight);
}
