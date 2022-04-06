using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField, Range(30, 179)] private float _verticalAngle;

    private bool _verticalInvertion = false;
    private float _xAxis = 0f;
    private float _turnSpeed = 0.2f;

    private const string INVERTION_PREF = "inv";
    private const string SENSITIVITY_PREF = "sens";

    void Awake()
    {
        if (PlayerPrefs.HasKey(INVERTION_PREF))
            UpdateVerticalInversion(PlayerPrefs.GetInt(INVERTION_PREF) == 1, false);
        else
            PlayerPrefs.SetInt(INVERTION_PREF, 0);

        if (PlayerPrefs.HasKey(SENSITIVITY_PREF))
            UpdateSensitivity(PlayerPrefs.GetFloat(SENSITIVITY_PREF), false);
        else
            PlayerPrefs.SetFloat(SENSITIVITY_PREF, _turnSpeed);
    }

    void Update()
    {
        Vector2 rawLookVector = PlayerInputController.GetLookVector(_playerInput);
        Vector2 lookVector = new Vector2(rawLookVector.x * _turnSpeed, rawLookVector.y * _turnSpeed);

        _xAxis = Mathf.Clamp(_xAxis + (_verticalInvertion ? lookVector.y : -lookVector.y), -_verticalAngle / 2, _verticalAngle / 2);
        transform.Rotate(Vector3.up * lookVector.x);

        Camera.main.transform.localRotation = Quaternion.Euler(Vector3.right * _xAxis);
    }

    public void UpdateVerticalInversion(bool isInverted, bool setPref = true)
    {
        _verticalInvertion = isInverted;
        if (setPref)
            PlayerPrefs.SetInt(INVERTION_PREF, isInverted ? 1 : 0);
    }

    public void UpdateSensitivity(float sensitivity, bool setPref = true)
    {
        _turnSpeed = sensitivity;
        if (setPref)
            PlayerPrefs.SetFloat(SENSITIVITY_PREF, sensitivity);
    }

    public bool GetVerticalInversion() => _verticalInvertion;

    public float GetSensitivity() => _turnSpeed;
}
