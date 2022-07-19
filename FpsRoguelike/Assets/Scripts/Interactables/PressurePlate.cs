using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public bool Interactable;

    public UnityEvent OnPlatePressed;
    public UnityEvent OnPlateReleased;

    [SerializeField] private GameObject _plate;

    private bool _isPressed = false;
    private HashSet<Transform> _standOnObjects;
    private Coroutine _movePlateCoroutine;

    private const float SPEED = 0.06f;
    private const float RELEASED_POS_Z = 0f;
    private const float PRESSED_POS_Z = -0.03f;

    void Awake()
    {
        _standOnObjects = new HashSet<Transform>();
        OnPlatePressed ??= new UnityEvent();
        OnPlateReleased ??= new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Interactable)
            return;

        if (_standOnObjects.Count == 0)
        {
            _isPressed = true;
            OnPlatePressed.Invoke();
            if (_movePlateCoroutine != null)
                StopCoroutine(_movePlateCoroutine);
            _movePlateCoroutine = StartCoroutine(MovePlate());
        }

        _standOnObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Interactable)
            return;
        
        _standOnObjects.Remove(other.transform);

        if (_standOnObjects.Count == 0)
        {
            _isPressed = false;
            OnPlateReleased.Invoke();
            if (_movePlateCoroutine != null)
                StopCoroutine(_movePlateCoroutine);
            _movePlateCoroutine = StartCoroutine(MovePlate());
        }
    }

    private IEnumerator MovePlate()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_isPressed? _plate.transform.localPosition.z > PRESSED_POS_Z
            : _plate.transform.localPosition.z < RELEASED_POS_Z)
        {
            Vector3 pos = _plate.transform.localPosition;
            float transition = SPEED * Time.deltaTime * (_isPressed? -1f : 1f);
            pos.z = Mathf.Clamp(pos.z + transition, PRESSED_POS_Z, RELEASED_POS_Z);
            _plate.transform.localPosition = pos;
            yield return wait;
        }
    }
}
