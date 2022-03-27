using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCrouching : MonoBehaviour
{
    public bool IsCrouched { get; private set; }

    [SerializeField, Min(0.01f)] private float _crouchStep;
    [SerializeField, Min(0)] private float _crouchedHeight;
    [SerializeField] private CapsuleCollider _capsuleCollider;

    private float _playerHeight;
    private float _cameraOffsetY;
    private Coroutine _sitCoroutine;

    void Start()
    {
        _playerHeight = _capsuleCollider.height;
        _cameraOffsetY = _playerHeight - Camera.main.transform.localPosition.y;
    }

    void Update()
    {
        // TODO: Create Input Manager
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_sitCoroutine != null)
                StopCoroutine(_sitCoroutine);
            _sitCoroutine = StartCoroutine(Sit(true));
        }    
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (_sitCoroutine != null)
                StopCoroutine(_sitCoroutine);
            _sitCoroutine = StartCoroutine(Sit(false));
        }
    }

    IEnumerator Sit(bool moveDown)
    {
        IsCrouched = true;
        var wait = new WaitForEndOfFrame();
        while (moveDown? _capsuleCollider.height > _crouchedHeight : _capsuleCollider.height < _playerHeight)
        {
            _capsuleCollider.height += (moveDown? -1 : 1) * _crouchStep * Time.deltaTime;
            _capsuleCollider.center = Vector3.up / 2 * _capsuleCollider.height;
            Camera.main.transform.localPosition = (_capsuleCollider.height - _cameraOffsetY) * Vector3.up;
            yield return wait;
        }
        _capsuleCollider.height = moveDown? _crouchedHeight : _playerHeight;
        _capsuleCollider.center = Vector3.up / 2 * _capsuleCollider.height;
        Camera.main.transform.localPosition = (_capsuleCollider.height - _cameraOffsetY) * Vector3.up;
        IsCrouched = !moveDown;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_capsuleCollider != null && _crouchedHeight > _capsuleCollider.height)
            _crouchedHeight = _capsuleCollider.height;
    }

#endif
}
