using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class Scanner : MonoBehaviour
{
    public UnityEventScanerView OnScannerViewEnter;
    public UnityEventScanerView OnScannerViewExit;

    [SerializeField, Min(0)] float _viewDistance;
    [SerializeField, Range(0, 360)] float _viewAngle;
    [SerializeField] GameObject _targetGameObject; // ебанный костыль

    IScanerTarget _target;

    private bool _isTargetWasVisible = false;

    void Awake()
    {
        OnScannerViewEnter ??= new UnityEventScanerView();
        OnScannerViewExit ??= new UnityEventScanerView();
    }

    void Start()
    {
        SetTarget(_targetGameObject?.GetComponent<IScanerTarget>());
    }

    void FixedUpdate()
    {
        float angle = Vector3.Angle(transform.forward, _target.transform.position - transform.position);
        
        if (angle <= _viewAngle / 2)
        {
            Vector3 direction = _target.transform.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, _viewDistance))
            {
                if (!_isTargetWasVisible && hit.transform == _target.transform)
                { 
                    _isTargetWasVisible = true;
                    OnScannerViewEnter.Invoke(_target);
                }
                else if (_isTargetWasVisible && hit.transform != _target.transform)
                {
                    _isTargetWasVisible = false;
                    OnScannerViewExit.Invoke(_target);
                }
            }
        }
        else if (_isTargetWasVisible && angle > _viewAngle / 2)
        {
            _isTargetWasVisible = false;
            OnScannerViewExit.Invoke(_target);
        }
    }

    public void SetTarget(IScanerTarget target)
    {
        _target = target;
        _isTargetWasVisible = false;
        OnScannerViewExit.Invoke(_target);
    }

    private void OnDestroy()
    {
        OnScannerViewExit.Invoke(_target);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _viewDistance);

        Gizmos.color = Color.blue;
        float yRotation = Mathf.Deg2Rad * transform.rotation.eulerAngles.y;
        float leftRad = Mathf.Deg2Rad * _viewAngle / 2 + yRotation;
        float rightRad = -Mathf.Deg2Rad * _viewAngle / 2 + yRotation;
        Vector3 leftAngle = new Vector3(Mathf.Sin(leftRad), 0, Mathf.Cos(leftRad)).normalized;
        Vector3 rightAngle = new Vector3(Mathf.Sin(rightRad), 0, Mathf.Cos(rightRad)).normalized;
        leftAngle *= _viewDistance;
        rightAngle *= _viewDistance;
        Gizmos.DrawLine(transform.position, transform.position + leftAngle);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle);

        if (_target == null || Vector3.Distance(transform.position, _target.transform.position) > _viewDistance)
            return;

        Gizmos.color = Color.red;
        float angle = Vector3.Angle(transform.forward, _target.Center - transform.position);
        if (angle <= _viewAngle / 2)
            Gizmos.DrawLine(transform.position, _target.Center);
    }

#endif
}