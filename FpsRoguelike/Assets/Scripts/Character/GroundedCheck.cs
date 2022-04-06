using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundedCheck : MonoBehaviour
{
    [Min(0), SerializeField] private float _rayLength;
    [SerializeField] private Vector3 _offset;

    [NonSerialized] public UnityEvent OnGetOffTheGround;
    [NonSerialized] public UnityEvent OnGetGrounded;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        if (OnGetOffTheGround == null)
            OnGetOffTheGround = new UnityEvent();
        if (OnGetGrounded == null)
            OnGetGrounded = new UnityEvent();
    }

    void Update()
    {
        Vector3 pos = GetRayStartPosition();
        bool temp = Physics.Raycast(pos, Vector3.down, _rayLength);

        if (temp && temp != IsGrounded)
        {
            IsGrounded = temp;
            OnGetGrounded.Invoke();
        }
        if (!temp && temp != IsGrounded)
        {
            IsGrounded = temp;
            OnGetOffTheGround.Invoke();
        }
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
