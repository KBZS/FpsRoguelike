using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFallJumpDuration : MonoBehaviour
{
    [SerializeField, Min(1)] private float _fallMultiplier = 1;
    [SerializeField, Min(1)] private float _lowJumpMultiplier = 1;
    [SerializeField] private Rigidbody _rigidbody;


    void Update()
    {
        if (_rigidbody.velocity.y < 0 && !Input.GetKey(KeyCode.Space))
            _rigidbody.velocity += Vector3.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        else if (_rigidbody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            _rigidbody.velocity += Vector3.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
