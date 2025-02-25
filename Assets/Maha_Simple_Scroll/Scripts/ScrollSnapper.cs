using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Regulates the snapping effect of the slider
/// by Maha
/// </summary>
[RequireComponent(typeof(SimpleScroll))]
public class ScrollSnapper : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private ScrollArranger _arranger;
    private SimpleScroll.ScrollDirection _direction;
    private bool _centerElements;
    private Action<float[]> _onSnapPointsCalculated;
    private float[] _snapPositions;

    public void Init(ScrollRect scrollRect, ScrollArranger arranger,
                     SimpleScroll.ScrollDirection direction,
                     bool center,
                     Action<float[]> onSnapPointsCalculated)
    {
        _scrollRect = scrollRect;
        _arranger = arranger;
        _direction = direction;
        _centerElements = center;
        _onSnapPointsCalculated = onSnapPointsCalculated;
    }

    public void InitializeSnapPoints()
    {
        RectTransform[] elements = ValidateElements();
        if (elements == null) return;

        float contentSize = CalculateContentSize();
        if (contentSize < 1f) contentSize = 1f;

        int count = elements.Length;
        _snapPositions = new float[count];
        for (int i = 0; i < count; i++)
        {
            float elementSize = GetElementSize(elements[i]);
            float margin = _centerElements ? GetCenterMargin(elementSize) : 0f;

            if (_direction == SimpleScroll.ScrollDirection.Horizontal)
            {
                _snapPositions[i] = ComputeHorizontalSnap(elements[i], margin, contentSize);
            }
            else
            {
                _snapPositions[i] = ComputeVerticalSnap(elements[i], margin, contentSize);
            }
        }

        _onSnapPointsCalculated?.Invoke(_snapPositions);
    }

    public float GetSnapPosition(int index)
    {
        if (_snapPositions == null || index < 0 || index >= _snapPositions.Length) return 0f;
        return _snapPositions[index];
    }

    public int FindClosestIndex(float normalized)
    {
        if (_snapPositions == null || _snapPositions.Length == 0) return 0;
        float closestDist = Mathf.Infinity;
        int closestIdx = 0;

        for (int i = 0; i < _snapPositions.Length; i++)
        {
            float dist = Mathf.Abs(normalized - _snapPositions[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIdx = i;
            }
        }
        return closestIdx;
    }

    public int GetTotalSlides()
    {
        return _snapPositions == null ? 0 : _snapPositions.Length;
    }

    /// <summary>
    /// Validates if we have elements to snap to. Logs error if none.
    /// </summary>
    private RectTransform[] ValidateElements()
    {
        RectTransform[] elements = _arranger.GetElements();
        if (elements == null || elements.Length == 0)
        {
            Debug.LogError("ScrollSnapper: No elements to snap.");
            return null;
        }
        return elements;
    }

    /// <summary>
    /// Calculates scrollable content size (width or height minus viewport).
    /// </summary>
    private float CalculateContentSize()
    {
        bool isHorizontal = _direction == SimpleScroll.ScrollDirection.Horizontal;
        float contentSize = isHorizontal
            ? _scrollRect.content.rect.width - _scrollRect.viewport.rect.width
            : _scrollRect.content.rect.height - _scrollRect.viewport.rect.height;

        return contentSize;
    }

    /// <summary>
    /// Returns element width/height depending on scroll direction.
    /// </summary>
    private float GetElementSize(RectTransform el)
    {
        return (_direction == SimpleScroll.ScrollDirection.Horizontal)
            ? el.rect.width
            : el.rect.height;
    }

    /// <summary>
    /// Calculates margin for centering the element if requested.
    /// </summary>
    private float GetCenterMargin(float elementSize)
    {
        float viewportSize = (_direction == SimpleScroll.ScrollDirection.Horizontal)
            ? _scrollRect.viewport.rect.width
            : _scrollRect.viewport.rect.height;

        return 0.5f * (viewportSize - elementSize);
    }

    /// <summary>
    /// Computes the normalized position for a horizontal element.
    /// </summary>
    private float ComputeHorizontalSnap(RectTransform el, float margin, float contentSize)
    {
        float xPos = el.anchoredPosition.x - margin;
        return Mathf.Clamp01(xPos / contentSize);
    }

    /// <summary>
    /// Computes the normalized position for a vertical element (top=1, bottom=0).
    /// </summary>
    private float ComputeVerticalSnap(RectTransform el, float margin, float contentSize)
    {
        float yPos = -el.anchoredPosition.y - margin;
        float ratio = yPos / contentSize;
        // top => ratio ~ 0 => invert => 1f - ratio
        return 1f - Mathf.Clamp01(ratio);
    }
}
