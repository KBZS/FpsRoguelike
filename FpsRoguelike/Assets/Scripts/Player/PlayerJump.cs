using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundedCheck)), RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerInputController _inputController;

    [SerializeField, Min(0)] private float _jumpHeight;
    [SerializeField, Min(0)] private int _maxJumpsCount;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GroundedCheck _groundedCheck;

    private float _jumpForce;
    private int _leftJumpsCount;

    void Start()
    {
        _jumpForce = CalculateJumpForce();
        RefreshLeftJumpsCount();
        _groundedCheck.OnGetGrounded.AddListener(RefreshLeftJumpsCount);
    }

    void Update()
    {
        if (_leftJumpsCount > 0 && _inputController.GetJumpPressedDown())
        {
            _leftJumpsCount--;
            Debug.Log("JUMP! " + _leftJumpsCount);

            Vector3 velocity = _rigidbody.velocity;
            velocity.y = 0.0f;
            _rigidbody.velocity = velocity;

            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void RefreshLeftJumpsCount() => _leftJumpsCount = _maxJumpsCount;

    private float CalculateJumpForce()
        => _rigidbody.mass * Mathf.Sqrt(2 *_jumpHeight * Physics.gravity.magnitude);

#if UNITY_EDITOR

    private void OnValidate()
    {
        _jumpForce = CalculateJumpForce();
    }

#endif
}
