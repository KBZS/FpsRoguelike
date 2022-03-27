using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    [Min(0), SerializeField] private float _rayLength;
    [SerializeField] private Vector3 _offset;

    void FixedUpdate()
    {
        Vector3 pos = GetRayStartPosition();
        IsGrounded = Physics.Raycast(pos, Vector3.down, _rayLength);
    }

    Vector3 GetRayStartPosition() => transform.position + _offset;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pos = GetRayStartPosition();
        Gizmos.DrawLine(pos, pos + Vector3.down * _rayLength);
    }

#endif
}
