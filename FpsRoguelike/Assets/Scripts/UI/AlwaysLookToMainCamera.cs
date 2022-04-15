using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookToMainCamera : MonoBehaviour
{
    private Transform _camera;

    void Start()
    {
        _camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(_camera);
        transform.Rotate(Vector3.up * 180);
    }
}
