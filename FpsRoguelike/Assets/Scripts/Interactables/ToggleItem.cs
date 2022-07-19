using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleItem : MonoBehaviour, IInteractable
{
    public bool Interactable;
    public bool IsOn;
    public string Info => _info;

    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    [SerializeField] private Material _onMaterial;
    [SerializeField] private Material _offMaterial;
    [SerializeField] private Transform _rotationObject;
    [SerializeField] private MeshRenderer _handle;
    [SerializeField] private string _info;
    
    private Coroutine _ñhangeStateCoroutine;

    private const float SPEED = 80f;
    private const float ON_ANGLE = 20f;
    private const float OFF_ANGLE = -20f;

    void Awake()
    {
        OnToggleOn ??= new UnityEvent();
        OnToggleOff ??= new UnityEvent();
        _ñhangeStateCoroutine = StartCoroutine(ChangeState());
    }

    public void DoAction()
    {
        if (!Interactable)
            return;

        if (IsOn)
            OnToggleOff.Invoke();
        else
            OnToggleOn.Invoke();

        IsOn = !IsOn;
        if (_ñhangeStateCoroutine != null)
            StopCoroutine(_ñhangeStateCoroutine);
        _ñhangeStateCoroutine = StartCoroutine(ChangeState());
    }

    private IEnumerator ChangeState()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        _handle.material = IsOn ? _onMaterial : _offMaterial;

        while (IsOn ? _rotationObject.localRotation.x < ON_ANGLE
            : _rotationObject.localRotation.x > OFF_ANGLE)
        {
            Vector3 euler = _rotationObject.localRotation.eulerAngles;
            float deltaAngle = Time.deltaTime * SPEED;
            if (euler.x > 180)
                euler.x -= 360;
            euler.x = Mathf.Clamp(euler.x + (IsOn ? deltaAngle : -deltaAngle), OFF_ANGLE, ON_ANGLE);
            _rotationObject.transform.localRotation = Quaternion.Euler(euler);
            yield return wait;
        }
    }
}
