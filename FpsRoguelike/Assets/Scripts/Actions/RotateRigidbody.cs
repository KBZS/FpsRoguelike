using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRigidbody : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;

    [SerializeField, Min(0)] float _speed;
    [SerializeField] Vector3 _rotation;

    private Vector3 _leftRotation;

    void FixedUpdate()
    {
        if (_leftRotation == Vector3.zero)
            return;

        Vector3 deltaRotation = _rotation * Time.fixedDeltaTime;
        if (_leftRotation.magnitude < deltaRotation.magnitude)
        { 
            deltaRotation = _leftRotation;
            _leftRotation = Vector3.zero;
        }
        else
            _leftRotation -= deltaRotation;

        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(deltaRotation));
    }

    public void Rotate()
    {
        _leftRotation += _rotation;
    }
}
