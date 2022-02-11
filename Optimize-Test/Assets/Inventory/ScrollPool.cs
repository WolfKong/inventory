using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ScrollRect))]
public class ScrollPool : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private RectTransform _content;

    private int _itemCount;
    private int _topIndex;
    private int _bottomIndex;

    private float _cellHeight;
    private float _viewPortHeight;

    private RectTransform[] _items;
    private ObjectPool _pool;
    private Action<GameObject, int> _initializeItem;

    public void Initialize<T>(T prefab, Action<GameObject, int> initializeItem, int itemCount) where T : Component
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(OnValueChanged);

        _viewPortHeight = _scrollRect.viewport.rect.height;
        _content = _scrollRect.content;

        _itemCount = itemCount;
        _initializeItem = initializeItem;

        _items = new RectTransform[_itemCount];

        var prefabRect = prefab.GetComponent<RectTransform>();
        _cellHeight = prefabRect.rect.height;

        var contentHeight = itemCount * _cellHeight;
        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

        var visibleSize = Mathf.CeilToInt(_viewPortHeight / _cellHeight);

        _pool = new ObjectPool();
        _pool.PopulatePool(prefab.gameObject, visibleSize + 2);

        for (int i = 0; i < visibleSize; i++)
        {
            CreateObjectAt(i);
        }

        _topIndex = CellIndexForPosition(_content.anchoredPosition.y);
        _bottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);
    }

    private void CreateObjectAt(int index)
    {
        var item = _pool.GetObject();
        item.transform.SetParent(_content);

        _initializeItem(item, index);
        var rectTransform = item.GetComponent<RectTransform>();
        SetCellPosition(rectTransform, index);
        _items[index] = rectTransform;
    }

    public void OnValueChanged(Vector2 position)
    {
        OnScroll();
    }

    public void OnScroll()
    {
        var newTopIndex = CellIndexForPosition(_content.anchoredPosition.y);
        var newBottomIndex = CellIndexForPosition(_content.anchoredPosition.y + _viewPortHeight);

        // Don't change items on scroll limits
        if (newTopIndex < 0 || newBottomIndex >= _itemCount)
            return;

        // Remove no longer visible items
        while (newTopIndex > _topIndex)
        {
            _pool.ReturnObject(_items[_topIndex].gameObject);
            _topIndex++;
        }

        while (newBottomIndex < _bottomIndex)
        {
            _pool.ReturnObject(_items[_bottomIndex].gameObject);
            _bottomIndex--;
        }

        // Add new visible items
        while (newTopIndex < _topIndex)
        {
            _topIndex--;
            CreateObjectAt(_topIndex);
        }

        while (newBottomIndex > _bottomIndex)
        {
            _bottomIndex++;
            CreateObjectAt(_bottomIndex);
        }
    }

    private void SetCellPosition(RectTransform rectTransform, int index) =>
        rectTransform.anchoredPosition = new Vector2(0, -index * _cellHeight);

    private int CellIndexForPosition(float position) => Mathf.FloorToInt(position / _cellHeight);
}
