using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GroundedCheck), typeof(PlayerCrouching), typeof(Rigidbody))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [Header("SPEED")]
    [SerializeField, Min(0)] private float _walkSpeed;
    [SerializeField, Min(0)] private float _runSpeed;
    [SerializeField, Min(0)] private float _speedInAir;
    [SerializeField, Min(0)] private float _crouchSpeed;
    
    [Space(20)]
    [SerializeField] private GroundedCheck _groundedCheck;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerCrouching _playerCrouching;

    private Vector3 _impulse;

    void Update()
    {
        Vector2 inputVector = PlayerInputController.GetMoveVector(_playerInput);
        _impulse = new Vector3(inputVector.x, 0.0f, inputVector.y).normalized;

        if (_groundedCheck.IsGrounded)
        {
            if (_playerCrouching.IsCrouched)
                _impulse *= _crouchSpeed;
            else
                _impulse *= PlayerInputController.GetRunHold(_playerInput) ? _runSpeed : _walkSpeed;
        }
        else
            _impulse *= _speedInAir;
    }

    void FixedUpdate() => _rigidbody.AddRelativeForce(_impulse, ForceMode.Impulse);

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_walkSpeed > _runSpeed)
            _walkSpeed = _runSpeed;

        if (_crouchSpeed > _walkSpeed)
            _crouchSpeed = _walkSpeed;
    }

#endif
}
