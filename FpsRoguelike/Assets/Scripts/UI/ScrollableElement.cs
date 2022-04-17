using System;
using UnityEngine;
using UnityEngine.Events;

public class ScrollableElement : MonoBehaviour
{
    public RectTransform parentRect = null;

    [NonSerialized] public int index = 0;

    [SerializeField] private UnityEvent _onAppear = null;
    [SerializeField] private UnityEvent _onCreation = null;
    
    public virtual void SetContent(object content) { }

    public void SetAppeared() => CallEvent(_onAppear);

    public void SetCreated() => CallEvent(_onCreation);

    public virtual void OnClick(Vector3 position) { }

    protected void CallEvent(UnityEvent eventToCall)
    {
#if UNITY_EDITOR
        try
        {
#endif
            eventToCall?.Invoke();
#if UNITY_EDITOR
        }
        catch (Exception e)
        {
            Debug.LogError(gameObject + " Threw an error!", gameObject);
            Debug.LogError(e.ToString());
        }
#endif
    }
}