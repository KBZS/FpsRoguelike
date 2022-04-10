using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private const float MAX_LASER_LENGTH = 100f;

    void Awake()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[2]);
    }

    void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, MAX_LASER_LENGTH))
            _lineRenderer.SetPosition(1, hit.point);
        else
            _lineRenderer.SetPosition(1, transform.forward * MAX_LASER_LENGTH + transform.position);
    }
}
