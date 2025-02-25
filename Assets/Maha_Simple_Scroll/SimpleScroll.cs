using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using R3;

[RequireComponent(typeof(ScrollRect), typeof(ScrollArranger), typeof(ScrollSnapper))]
public class SimpleScroll : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public enum ScrollDirection { Horizontal, Vertical }

    [Header("References")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private ScrollArranger _arranger;
    [SerializeField] private ScrollSnapper _snapper;

    [Header("Settings")]
    [SerializeField] private ScrollDirection _scrollDirection = ScrollDirection.Horizontal;
    [SerializeField] private bool _enableSnapping = true;
    [SerializeField] private float _snapSpeed = 10f;
    [SerializeField] private bool _centerElements = false;
    [SerializeField] private float _itemSpacing = 20f;

    [Header("Options")]
    [SerializeField] private bool _useNavigationButtons = false;
    [SerializeField] private bool _useParallaxEffect = false;
    [SerializeField] private bool _useToggleNavigation = false;

    private bool _isSnapping = false;
    private int _currentIndex = 0;

    private Subject<int> _onSlideChanged = new();
    public Observable<int> OnSlideChangedObservable => _onSlideChanged.AsObservable();

    public delegate void SlideChanged(int index);
    public event SlideChanged OnSlideChangedEvent;

    private void Awake()
    {
        if (!_scrollRect) _scrollRect = GetComponent<ScrollRect>();
        if (!_arranger) _arranger = GetComponent<ScrollArranger>();
        if (!_snapper) _snapper = GetComponent<ScrollSnapper>();
    }

    private void Start()
    {
        _arranger.Init(_scrollRect, _scrollDirection, _centerElements, _itemSpacing);
        _arranger.ArrangeElements();

        if (_arranger.GetElementCount() == 0)
        {
            Debug.LogWarning("SimpleScroll: No elements found.");
            return;
        }

        _snapper.Init(_scrollRect, _arranger, _scrollDirection, _centerElements, OnSnapPointsReady);
        _snapper.InitializeSnapPoints();
    }

    /// <summary>
    /// Callback when snap points are ready. Set initial position here.
    /// </summary>
    private void OnSnapPointsReady(float[] snapPositions)
    {
        _scrollRect.horizontal = _scrollDirection == ScrollDirection.Horizontal;
        _scrollRect.vertical = _scrollDirection == ScrollDirection.Vertical;

        if (snapPositions.Length > 0)
        {
            float initialPos = snapPositions[0];
            if (_scrollDirection == ScrollDirection.Horizontal)
                _scrollRect.horizontalNormalizedPosition = initialPos;
            else
                _scrollRect.verticalNormalizedPosition = initialPos;
        }
    }

    // Unity events
    public void OnDrag(PointerEventData eventData)
    {
        _isSnapping = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_enableSnapping) return;
        float currentNorm = GetNormalizedPosition();

        _currentIndex = _snapper.FindClosestIndex(currentNorm);
        _isSnapping = true;
        _onSlideChanged.OnNext(_currentIndex);
    }

    private void Update()
    {
        if (_isSnapping)
        {
            float targetPos = _snapper.GetSnapPosition(_currentIndex);
            float newPos = Mathf.Lerp(GetNormalizedPosition(), targetPos, Time.deltaTime * _snapSpeed * 2f);
            SetNormalizedPosition(newPos);

            if (Mathf.Abs(GetNormalizedPosition() - targetPos) < 0.001f)
            {
                _isSnapping = false;
            }
        }
    }

    public void GoToSlide(int index)
    {
        if (index >= 0 && index < _snapper.GetTotalSlides())
        {
            _currentIndex = index;
            _isSnapping = true;
            _onSlideChanged.OnNext(_currentIndex);
        }
    }

    public void GoToNextSlide()
    {
        if (_currentIndex < _snapper.GetTotalSlides() - 1)
        {
            _currentIndex++;
            _isSnapping = true;
            _onSlideChanged.OnNext(_currentIndex);
        }
    }

    public void GoToPreviousSlide()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            _isSnapping = true;
            _onSlideChanged.OnNext(_currentIndex);
        }
    }

    public int GetCurrentIndex() => _currentIndex;
    public int GetTotalSlides() => _snapper.GetTotalSlides();

    private float GetNormalizedPosition()
    {
        return _scrollDirection == ScrollDirection.Horizontal
            ? _scrollRect.horizontalNormalizedPosition
            : _scrollRect.verticalNormalizedPosition;
    }

    private void SetNormalizedPosition(float value)
    {
        if (_scrollDirection == ScrollDirection.Horizontal)
            _scrollRect.horizontalNormalizedPosition = value;
        else
            _scrollRect.verticalNormalizedPosition = value;
    }
}
