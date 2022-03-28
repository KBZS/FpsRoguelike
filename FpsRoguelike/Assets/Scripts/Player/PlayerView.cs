using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField, Range(30, 179)] private float _verticalAngle;
    [SerializeField, Min(0.001f)] private float _horizontalTurnSpeed;
    [SerializeField, Min(0.001f)] private float _verticalTurnSpeed;

    private float _xAxis = 0f;

    void Update()
    {
        float horizontalTurn = Input.GetAxis("Mouse X") * _horizontalTurnSpeed;
        float verticalTurn = Input.GetAxis("Mouse Y") * _verticalTurnSpeed * -1;
        _xAxis = Mathf.Clamp(_xAxis + verticalTurn, -_verticalAngle / 2, _verticalAngle / 2);

        transform.Rotate(Vector3.up * horizontalTurn);
        Camera.main.transform.localRotation = Quaternion.Euler(Vector3.right * _xAxis);
    }
}
