using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField, Range(30, 179)] private float _verticalAngle;
    [SerializeField, Min(0.001f)] private float _horizontalTurnSpeed = 0.2f;
    [SerializeField, Min(0.001f)] private float _verticalTurnSpeed = 0.2f;

    private bool _verticalInvertion = false;
    private float _xAxis = 0f;

    void Update()
    {
        Vector2 rawLookVector = PlayerInputController.GetLookVector(_playerInput);
        Vector2 lookVector = new Vector2(rawLookVector.x * _horizontalTurnSpeed, rawLookVector.y * _verticalTurnSpeed);

        _xAxis = Mathf.Clamp(_xAxis + (_verticalInvertion ? lookVector.y : -lookVector.y), -_verticalAngle / 2, _verticalAngle / 2);
        transform.Rotate(Vector3.up * lookVector.x);

        Camera.main.transform.localRotation = Quaternion.Euler(Vector3.right * _xAxis);
    }

    public void UpdateVerticalInversion(bool isInverted) => _verticalInvertion = isInverted;
}
