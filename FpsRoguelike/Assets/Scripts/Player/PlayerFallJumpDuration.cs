using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFallJumpDuration : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField, Min(1)] private float _fallMultiplier = 1;
    [SerializeField, Min(1)] private float _lowJumpMultiplier = 1;
    [SerializeField] private Rigidbody _rigidbody;

    void Update()
    {
        bool isReleased = !PlayerInputController.GetJumpHold(_playerInput);

        if (_rigidbody.velocity.y < 0 && isReleased)
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        else if (_rigidbody.velocity.y > 0 && isReleased)
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
