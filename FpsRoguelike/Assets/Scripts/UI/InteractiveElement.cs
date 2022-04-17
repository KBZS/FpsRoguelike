using UnityEngine.EventSystems;
using UnityEngine;

public class InteractiveElement : MonoBehaviour
    , IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected RectTransform rect = null;
    protected bool buttonDown = false;
    protected PointerEventData eventData;

    private bool _inside = false;

    protected virtual void Awake()
    {
        if (!GetComponent<CanvasRenderer>())
            gameObject.AddComponent<CanvasRenderer>();

        rect = (RectTransform)transform;
    }

    protected virtual void OnDisable()
    {
        buttonDown = false;
        eventData = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.eventData != null)
            return;

        _inside = true;
        buttonDown = true;
        this.eventData = eventData;
        PointerDownEvent(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.eventData == null || this.eventData != eventData)
            return;

        buttonDown = false;
        this.eventData = null;
        PointerUpEvent(eventData);
        PointerUpEvent(eventData, _inside);
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus || eventData == null)
            return;

        _inside = false;
        PointerUpEvent(eventData);
        buttonDown = false;
        eventData = null;
    }

    public void OnPointerExit(PointerEventData eventData) => _inside = false;

    public void OnPointerEnter(PointerEventData eventData) => _inside = true;

    protected virtual void PointerDownEvent(PointerEventData eventData) { }

    protected virtual void PointerUpEvent(PointerEventData eventData) { }

    protected virtual void PointerUpEvent(PointerEventData eventData, bool inside) { }
}