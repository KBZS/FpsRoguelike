using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpDownRigidbody : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;

    [SerializeField] float _maxY;
    [SerializeField] float _minY;
    [SerializeField, Min(0)] float _speed;

    private bool _moveUp;

    public void MoveUp()
    {
        _moveUp = true;
    }

    public void MoveDown()
    {
        _moveUp = false;
    }

    void FixedUpdate()
    {
        if (_moveUp ? transform.localPosition.y < _maxY : transform.localPosition.y > _minY)
        {
            float deltaY = _speed * Time.fixedDeltaTime;
            deltaY = _moveUp ? Mathf.Min(_maxY - transform.localPosition.y, deltaY)
                : Mathf.Min(transform.localPosition.y - _minY, deltaY);
            _rigidbody.MovePosition(transform.position + (_moveUp ? Vector3.up : Vector3.down) * deltaY);
        }
    }
}
