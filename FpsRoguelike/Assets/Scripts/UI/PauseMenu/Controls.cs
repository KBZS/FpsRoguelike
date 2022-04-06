using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollerContent;
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private InputField _sensitivityInput;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private ToggleController _invertionToggle;

    [Header("Texts")]
    [SerializeField] private Text _title;
    [SerializeField] private Text _sensitivityTitle;
    [SerializeField] private Text _invertionTitle;

    private float _initialFloatValue = 0.0f;

    private const float MAX_SENS = 1.0f;
    private const float MIN_SENS = 0.01f;

    void OnEnable()
    {
        ResetScrollerPosition();
        SetTexts();

        _sensitivityInput.contentType = InputField.ContentType.DecimalNumber;
        SetSensitivity();
        SetInvertion();
    }

    void ResetScrollerPosition() => _scrollerContent.anchoredPosition = new Vector2(_scrollerContent.anchoredPosition.x, 0.0f);

    void SetTexts()
    {
        _title.text = "Controls";
        _sensitivityTitle.text = "Sensitivity";
        _invertionTitle.text = "Vertical invertion";
    }

    void SetInvertion()
    {
        _invertionToggle.OnFalse = () => _playerView.UpdateVerticalInversion(false);
        _invertionToggle.OnTrue = () => _playerView.UpdateVerticalInversion(true);
        _invertionToggle.SetToggle(_playerView.GetVerticalInversion());
    }


    void SetSensitivity()
    {
        _initialFloatValue = _playerView.GetSensitivity();
        string stringValue = GetString(_initialFloatValue);

        _sensitivitySlider.SetValueWithoutNotify(_initialFloatValue);
        _sensitivitySlider.onValueChanged.AddListener(delegate { UpdateSensitivity(true); });

        _sensitivityInput.SetTextWithoutNotify(stringValue);
        _sensitivityInput.onValueChanged.AddListener(delegate { UpdateSensitivity(false); });
        _sensitivityInput.onEndEdit.AddListener(delegate { InputFinalCheck(); });
    }

    void UpdateSensitivity(bool usingSlider)
    {
        float newValue = 0.0f;

        if (usingSlider)
        {
            newValue = BoundariesCheck(_sensitivitySlider.value);
            _sensitivityInput.text = GetString(newValue);
        }
        else if (ParseFloat(_sensitivityInput.text.Replace(",", "."), out float parsedValue))
            newValue = BoundariesCheck(parsedValue);
        else 
            return;

        _playerView.UpdateSensitivity(newValue);
    }

    void InputFinalCheck()
    {
        if (ParseFloat(_sensitivityInput.text.Replace(",", "."), out float parsedValue))
        {
            float tempValue = BoundariesCheck(parsedValue);
            _sensitivitySlider.value = tempValue;
            _sensitivityInput.text = GetString(tempValue);
            _playerView.UpdateSensitivity(tempValue);
        }
    }

    bool ParseFloat(string stringValue, out float parsedValue)
    {
        NumberStyles style = NumberStyles.Float;
        CultureInfo culture = CultureInfo.InvariantCulture;
        return float.TryParse(stringValue, style, culture, out parsedValue);
    }

    string GetString(float value) => value.ToString("0.00", CultureInfo.CreateSpecificCulture("en-EN"));

    float BoundariesCheck(float value) => value > MAX_SENS ? MAX_SENS : value < MIN_SENS ? MIN_SENS : value;

}
