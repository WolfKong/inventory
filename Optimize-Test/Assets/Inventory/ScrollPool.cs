using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ScrollRect))]
public class ScrollPool : MonoBehaviour
{
    private RectTransform _content;
    private GameObject[] _items;
    private ObjectPool _pool;
    private Action<GameObject, int> _initializeItem;

    private int _itemCount;
    private int _topIndex;
    private int _bottomIndex;

    private float _cellHeight;
    private float _viewPortHeight;

    /// <summary>
    /// Set sizes, creates pool of items and displays visible ones.
    /// </summary>
    /// <param name="prefab">Prefab to generate items.</param>
    /// <param name="initializeItem">Action that set's up item data.</param>
    /// <param name="itemCount">Total item count.</param>
    public void Initialize<T>(T prefab, Action<GameObject, int> initializeItem, int itemCount) where T : Component
    {
        var scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnValueChanged);

        _itemCount = itemCount;
        _initializeItem = initializeItem;

        _cellHeight = prefab.GetComponent<RectTransform>().rect.height;
        _viewPortHeight = scrollRect.viewport.rect.height;

        _content = scrollRect.content;
        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemCount * _cellHeight);

        _topIndex = CellIndexForPosition(_content.anchoredPosition.y);
        _bottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);

        _pool = new ObjectPool();
        _pool.PopulatePool(prefab.gameObject, _content, _bottomIndex - _topIndex + 2);

        _items = new GameObject[_itemCount];

        for (int i = _topIndex; i <= _bottomIndex; i++)
            PlaceObjectAt(i);
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
            _topIndex++;
        }

        while (newBottomIndex < _bottomIndex)
        {
            _pool.ReturnObject(_items[_bottomIndex]);
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
