using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(SimpleScroll))]
public class ScrollArranger : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private SimpleScroll.ScrollDirection _direction;
    private bool _centerElements;
    private float _itemSpacing;
    private RectTransform[] _elements;

    public void Init(ScrollRect scrollRect, SimpleScroll.ScrollDirection direction, bool centerElements, float itemSpacing)
    {
        _scrollRect = scrollRect;
        _direction = direction;
        _centerElements = centerElements;
        _itemSpacing = itemSpacing;
    }

    public void ArrangeElements()
    {
        int count = _scrollRect.content.childCount;
        if (count == 0)
        {
            Debug.LogWarning("ScrollArranger: No child elements found in content.");
            return;
        }

        _elements = new RectTransform[count];
        bool isHorizontal = (_direction == SimpleScroll.ScrollDirection.Horizontal);

        float currentPosition = 0f;
        float totalElementSize = 0f;
        float totalSpacing = 0f;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = _scrollRect.content.GetChild(i) as RectTransform;
            _elements[i] = child;

            float elementSize = (isHorizontal) ? child.rect.width : child.rect.height;
            float margin = _centerElements ? CalculateMargin(GetViewportSize(), elementSize) : 0f;

            SetElementAnchors(child, isHorizontal);
            SetElementPosition(child, currentPosition, margin, isHorizontal);

            totalElementSize += elementSize;

            // Add spacing only if not the last element
            if (i < count - 1)
            {
                float spacing = _centerElements ? margin * 2 : _itemSpacing;
                totalSpacing += spacing;
                currentPosition += elementSize + spacing;
            }
            else
            {
                currentPosition += elementSize;
            }
        }

        float totalSize = totalElementSize + totalSpacing;
        // You can do your special hack for vertical sizing if needed
        // if (!isHorizontal) { totalSize -= (...) }

        // Finally, set the content size
        SetContentSize(totalSize, isHorizontal);
    }

    // Accessors
    public int GetElementCount() => (_elements == null) ? 0 : _elements.Length;
    public RectTransform[] GetElements() => _elements;

    // Private layout helpers
    private float GetViewportSize()
    {
        return (_direction == SimpleScroll.ScrollDirection.Horizontal)
            ? _scrollRect.viewport.rect.width
            : _scrollRect.viewport.rect.height;
    }

    private float CalculateMargin(float viewportSize, float elementSize)
    {
        return (viewportSize - elementSize) * 0.5f;
    }

    private void SetElementAnchors(RectTransform element, bool isHorizontal)
    {
        if (isHorizontal)
        {
            element.anchorMin = new Vector2(0f, 0.5f);
            element.anchorMax = element.anchorMin;
            element.pivot = element.anchorMin;
        }
        else
        {
            element.anchorMin = new Vector2(0.5f, 1f);
            element.anchorMax = element.anchorMin;
            element.pivot = element.anchorMin;
        }
    }

    private void SetElementPosition(RectTransform element, float currentPosition, float margin, bool isHorizontal)
    {
        if (isHorizontal)
            element.anchoredPosition = new Vector2(currentPosition + margin, 0f);
        else
            element.anchoredPosition = new Vector2(0f, -currentPosition - margin);
    }

    private void SetContentSize(float totalSize, bool isHorizontal)
    {
        if (isHorizontal)
            _scrollRect.content.sizeDelta = new Vector2(totalSize, _scrollRect.content.sizeDelta.y);
        else
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, totalSize);
    }
}