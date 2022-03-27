using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    [Min(0), SerializeField] private float RayLength;
    [SerializeField] private Vector3 Offset;

    void FixedUpdate()
    {
        Vector3 pos = GetRayStartPosition();
        IsGrounded = Physics.Raycast(pos, Vector3.down, RayLength);
    }

    Vector3 GetRayStartPosition() => transform.position + Offset;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pos = GetRayStartPosition();
        Gizmos.DrawLine(pos, pos + Vector3.down * RayLength);
    }

#endif
}
