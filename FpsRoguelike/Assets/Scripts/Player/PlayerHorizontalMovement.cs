using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundedCheck), typeof(PlayerCrouching), typeof(Rigidbody))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    [Header("SPEED")]
    [SerializeField, Min(0)] private float _walkSpeed;
    [SerializeField, Min(0)] private float _runSpeed;
    [SerializeField, Min(0)] private float _speedInAir;
    [SerializeField, Min(0)] private float _crouchSpeed;
    
    [Space(20)]
    [SerializeField] private GroundedCheck _groundedCheck;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerCrouching _playerCrouching;

    void Update()
    {
        float xTranslation = Input.GetAxis("Horizontal");
        float zTranslation = Input.GetAxis("Vertical");
        Vector3 translation = new Vector3(xTranslation, 0, zTranslation).normalized * Time.deltaTime;

        if (_groundedCheck.IsGrounded)
        { 
            if (_playerCrouching.IsCrouched)
                translation *= _crouchSpeed;
            else
                translation *= Input.GetKey(KeyCode.LeftShift)? _runSpeed : _walkSpeed;
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
