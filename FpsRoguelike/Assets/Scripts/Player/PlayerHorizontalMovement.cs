using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GroundedCheck), typeof(PlayerCrouching), typeof(Rigidbody))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputController _inputController;

    [Header("SPEED")]
    [SerializeField, Min(0)] private float _walkSpeed;
    [SerializeField, Min(0)] private float _runSpeed;
    [SerializeField, Min(0)] private float _speedInAir;
    [SerializeField, Min(0)] private float _crouchSpeed;
    
    [Space(20)]
    [SerializeField] private GroundedCheck _groundedCheck;
    [SerializeField] private PlayerCrouching _playerCrouching;

    void Update()
    {
        Vector2 inputVector = _inputController.GetMoveVector();
        Vector3 translation = new Vector3(inputVector.x, 0, inputVector.y).normalized * Time.deltaTime;

        if (_groundedCheck.IsGrounded)
        {
            if (_playerCrouching.IsCrouched)
                translation *= _crouchSpeed;
            else
                translation *= _inputController.GetRunHold() ? _runSpeed : _walkSpeed;
        }
        else
            translation *= _speedInAir;

        transform.Translate(translation);
    }

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
