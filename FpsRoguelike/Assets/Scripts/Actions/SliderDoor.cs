using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderDoor : MonoBehaviour
{
    [SerializeField] Vector3 _openPosition;
    [SerializeField] Vector3 _closedPosition;
    [SerializeField, Min(0)] float _speed;

    private Coroutine _changeDoorPositionCoroutine;

    private const float INFELICITY = 0.01f;

    public void OpenDoor()
    {
        if (_changeDoorPositionCoroutine != null)
            StopCoroutine(_changeDoorPositionCoroutine);
        _changeDoorPositionCoroutine = StartCoroutine(ChangeDoorPosition(_openPosition));
    }

    public void CloseDoor()
    {
        if (_changeDoorPositionCoroutine != null)
            StopCoroutine(_changeDoorPositionCoroutine);
        _changeDoorPositionCoroutine = StartCoroutine(ChangeDoorPosition(_closedPosition));
    }

    private IEnumerator ChangeDoorPosition(Vector3 target)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (Vector3.Distance(transform.localPosition, target) >= INFELICITY)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, _speed * Time.deltaTime);
            yield return wait;
        }

        transform.localPosition = target;
    }
}
