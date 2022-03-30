using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFallJumpDuration : MonoBehaviour
{
    [SerializeField] private PlayerInputController _inputController;

    [SerializeField, Min(1)] private float _fallMultiplier = 1;
    [SerializeField, Min(1)] private float _lowJumpMultiplier = 1;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerInput _playerInput;

    void Update()
    {
        bool isReleased = !_inputController.GetJumpHold();

        if (_rigidbody.velocity.y < 0 && isReleased)
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        else if (_rigidbody.velocity.y > 0 && isReleased)
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
