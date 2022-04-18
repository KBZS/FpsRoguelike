using System;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventStealth : UnityEvent<StealthEventArgs> { }

public class StealthEventArgs : EventArgs
{
    public Transform Sender;
    public Transform Target;
    public float ReactionTime;
    public float ForgetTargetTime;
}
