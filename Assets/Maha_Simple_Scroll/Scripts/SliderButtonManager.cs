
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections;

/// <summary>
/// Manages navigation buttons for a SimpleScroll component.
/// by Maha
/// </summary>
[RequireComponent(typeof(SimpleScroll))]
public sealed class SliderButtonManager : MonoBehaviour
{
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private SimpleScroll _customSlider;
    [SerializeField] private bool _useInteractableForPrevButton;
    [SerializeField] private bool _useInteractableForNextButton;

    private IEnumerator Start()
    {
        // to wait until slides are ready
        yield return new WaitUntil(() => _customSlider.GetTotalSlides() > 0);

        SetNavigationButtonsObservables();
        UpdateNavigationButtonsState(_customSlider.GetCurrentIndex());
    }

    /// <summary>
    /// Subscribes button clicks to navigation actions.
    /// </summary>
    private void SetNavigationButtonsObservables()
    {
        _prevButton?.OnClickAsObservable().Subscribe(_ => _customSlider.GoToPreviousSlide()).AddTo(this);
        _nextButton?.OnClickAsObservable().Subscribe(_ => _customSlider.GoToNextSlide()).AddTo(this);

        _customSlider.OnSlideChangedObservable.Subscribe(UpdateNavigationButtonsState).AddTo(this);
    }

    /// <summary>
    /// Updates button states based on the current slide index.
    /// </summary>
    private void UpdateNavigationButtonsState(int currentIndex)
    {
        int totalSlides = _customSlider.GetTotalSlides();

        bool isFirstSlide = (currentIndex == 0);
        bool isLastSlide = (currentIndex == totalSlides - 1);

        SetButtonState(_prevButton, _useInteractableForPrevButton, isFirstSlide);
        SetButtonState(_nextButton, _useInteractableForNextButton, isLastSlide);
    }

    private void SetButtonState(Button button, bool isInteractableInsteadOfActive, bool isBoundary)
    {
        if (isInteractableInsteadOfActive)
        {
            button.interactable = !isBoundary;
        }
        else
        {
            button.gameObject.SetActive(!isBoundary);
        }
    }
}

