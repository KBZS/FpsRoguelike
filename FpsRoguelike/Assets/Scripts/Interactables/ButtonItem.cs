using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonItem : MonoBehaviour, IInteractable
{
    public bool Interactable;

    public UnityEvent OnClick;

    public string Info => _info;

    [SerializeField] private string _info;
    [SerializeField] private Animation _animation;

    void Awake()
    {
        if (OnClick == null)
            OnClick = new UnityEvent();
    }

    public void DoAction()
    {
        if (!Interactable)
            return;

        OnClick.Invoke();
        _animation.Play();
    }
}
