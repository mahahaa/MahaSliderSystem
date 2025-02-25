using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections;

[RequireComponent(typeof(SimpleScroll))]
public class ToggleGroupManager : MonoBehaviour
{
    [SerializeField] private SimpleScroll _customSlider;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle _togglePrefab;

    private List<Toggle> _toggles = new();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => _customSlider.GetTotalSlides() > 0);

        InitializeToggles();
        _customSlider.OnSlideChangedObservable.Subscribe(UpdateToggleSelection).AddTo(this);
    }

    /// <summary>
    /// Creates toggles dynamically based on the number of slides.
    /// </summary>
    private void InitializeToggles()
    {
        ClearExistingToggles();
        int slideCount = _customSlider.GetTotalSlides();
        for (int i = 0; i < slideCount; i++)
        {
            Toggle newToggle = Instantiate(_togglePrefab, _toggleGroup.transform);
            newToggle.group = _toggleGroup;

            int index = i;
            newToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) _customSlider.GoToSlide(index);
            });

            _toggles.Add(newToggle);
        }

        if (_toggles.Count > 0)
        {
            _toggles[0].SetIsOnWithoutNotify(true);
        }
    }

    private void ClearExistingToggles()
    {
        foreach (var toggle in _toggles)
        {
            Destroy(toggle.gameObject);
        }
        _toggles.Clear();
    }

    /// <summary>
    /// Updates the selected toggle when the slide changes.
    /// </summary>
    private void UpdateToggleSelection(int currentIndex)
    {
        if (currentIndex >= 0 && currentIndex < _toggles.Count)
        {
            _toggles[currentIndex].SetIsOnWithoutNotify(true);
        }
    }
}