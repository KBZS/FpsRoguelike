using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTurretToTarget : MonoBehaviour
{
    public IScanerTarget Target { get; set;}

    [SerializeField, Range(-180, 0)] private float _minVerticalAngle = -35;
    [SerializeField, Range(0, 180)] private float _maxVerticalAngle = 33;
    [SerializeField, Range(0.1f, 50f)] private float _speed;   

    void Update()
    {
        if (Target == null)
            return;

        Vector3 targetDir = Target.Center - transform.position;

        Vector3 euler = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir),
            Time.deltaTime * _speed).eulerAngles;
        if (euler.x > 180)
            euler.x -= 360; 
        euler.x = Mathf.Clamp(euler.x, _minVerticalAngle, _maxVerticalAngle);
        euler.z = transform.rotation.z;

        transform.localRotation = Quaternion.Euler(euler);
    }
}
