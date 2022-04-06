using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _toggleImage;
    [SerializeField] private Image _handle;
    [SerializeField] private Color _onColorHandle = Color.green;
    [SerializeField] private Color _offColorHandle = Color.red;
    [SerializeField] private Color _onColorBackground = Color.grey;
    [SerializeField] private Color _offColorBackground = Color.grey;
    [SerializeField] private float _handleOffset = 2.0f;
    [SerializeField] private float _toggleDuration = 1.0f;

    public Action OnTrue { get; set; }
    public Action OnFalse { get; set; }

    private float _finalOffset = 0.0f;
    private bool _isOn = false;

    void Awake()
    {
        float handleSizeX = _handle.rectTransform.sizeDelta.x;
        float toggleSizeX = _toggleImage.rectTransform.sizeDelta.x;
        _finalOffset = (toggleSizeX / 2) - (handleSizeX / 2) - _handleOffset;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Toggle);
    }

    void OnEnable() => ForceState();

    void ForceState()
    {
        _toggleImage.color = _isOn ? _onColorBackground : _offColorBackground;
        _handle.color = _isOn ? _onColorHandle : _offColorHandle;
        _handle.rectTransform.anchoredPosition = new Vector2(_isOn ? _finalOffset : -_finalOffset, 0.0f);
    }

    void Toggle()
    {
        if (_isOn)
            OnFalse?.Invoke();
        else
            OnTrue?.Invoke();

        _isOn = !_isOn;

        StopAllCoroutines();
        StartCoroutine(ToggleCoroutine(_isOn));
    }

    IEnumerator ToggleCoroutine(bool toggleStatus)
    {
        float deltaTime = 0;
        float currentOffset = _handle.rectTransform.anchoredPosition.x;
        Color currentBackgroundColor = _toggleImage.color;
        Color currentHandleColor = _handle.color;

        Color targetBackgorundColor = toggleStatus ? _onColorBackground : _offColorBackground;
        Color targetHandleColor = toggleStatus ? _onColorHandle : _offColorHandle;
        float targetOffset = toggleStatus ? _finalOffset : -_finalOffset;

        while (deltaTime < _toggleDuration)
        {
            _toggleImage.color = Color.Lerp(currentBackgroundColor, targetBackgorundColor, deltaTime / _toggleDuration);
            _handle.color = Color.Lerp(currentHandleColor, targetHandleColor, deltaTime / _toggleDuration);
            _handle.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(currentOffset, targetOffset, deltaTime / _toggleDuration), 0f);
            deltaTime += Time.fixedDeltaTime;
            yield return null;
        }

        _toggleImage.color = targetBackgorundColor;
        _handle.color = targetHandleColor;
        _handle.rectTransform.anchoredPosition = new Vector2(targetOffset, 0.0f);
    }

    public void SetToggle(bool value)
    {
        StopAllCoroutines();
        _isOn = value;
        ForceState();
    }

    public bool GetToggleState() => _isOn;
}