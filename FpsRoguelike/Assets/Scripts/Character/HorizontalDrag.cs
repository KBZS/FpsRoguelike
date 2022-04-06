using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HorizontalDrag : MonoBehaviour
{
    [Range(0, 1)]public float DragFactor = 0.7f;
    [SerializeField] Rigidbody _rigidbody;

    void FixedUpdate()
    {
        Vector3 velocity = _rigidbody.velocity;
        velocity.x *= 1.0f - DragFactor;
        velocity.z *= 1.0f - DragFactor;
        _rigidbody.velocity = velocity;
    }
}
