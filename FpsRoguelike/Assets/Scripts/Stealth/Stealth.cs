using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour
{
    public UnityEventStealth OnGetWorried;
    public UnityEventStealth OnReact;
    public UnityEventStealth OnLoseTarget;
    public UnityEventStealth OnCalmDown;

    [SerializeField, Min(0)] private float _timeToReact;
    [SerializeField, Min(0)] private float _timeToForgetTarget;
    [SerializeField, Space(2)] private Scanner _scanner;

    private StealthEventArgs _args;
    private Transform _target;
    private bool _reacted = false;
    private float _woriedTimer = 0;

    private Coroutine _startWoriedTimerCoroutine;
    private Coroutine _endWoriedTimerCoroutine;

    void Awake()
    {
        if (OnGetWorried == null)
            OnGetWorried = new UnityEventStealth();
        if (OnReact == null)
            OnReact = new UnityEventStealth();
        if (OnLoseTarget == null)
            OnLoseTarget = new UnityEventStealth();
        if (OnCalmDown == null)
            OnCalmDown = new UnityEventStealth();
    }

    void Start()
    {
        _scanner.OnScannerViewEnter.AddListener(OnScannerViewEnter);
        _scanner.OnScannerViewExit.AddListener(OnScannerViewExit);
    }

    void OnScannerViewEnter(IScanerTarget target)
    {
        if (!_reacted)
        {
            _args = new StealthEventArgs
            {
                Sender = transform,
                Target = _target,
                ReactionTime = _timeToReact,
                ForgetTargetTime = _timeToForgetTarget
            };
            _target = target.transform;
            if (_endWoriedTimerCoroutine != null)
                StopCoroutine(_endWoriedTimerCoroutine);
            _startWoriedTimerCoroutine = StartCoroutine(StartWoriedTimer());
        }
        else
            CancelInvoke(nameof(CalmDown));
    }

    void OnScannerViewExit(IScanerTarget target)
    {
        if (!_reacted)
        { 
            _target = target.transform;
            if (_startWoriedTimerCoroutine != null)
                StopCoroutine(_startWoriedTimerCoroutine);
            _endWoriedTimerCoroutine = StartCoroutine(EndWoriedTimer());
        }
        else
            Invoke(nameof(CalmDown), _timeToForgetTarget);
    }

    IEnumerator StartWoriedTimer()
    {
        OnGetWorried.Invoke(_args);
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_woriedTimer < _timeToReact)
        { 
            _woriedTimer += Time.deltaTime;
            yield return wait;
        }
        _woriedTimer = _timeToReact;
        _reacted = true;
        OnReact.Invoke(_args);
    }

    IEnumerator EndWoriedTimer()
    {
        OnLoseTarget.Invoke(_args);
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_woriedTimer > 0)
        {
            _woriedTimer -= Time.deltaTime;
            yield return wait;
        }
        _woriedTimer = 0;
        OnCalmDown.Invoke(_args);
    }

    void CalmDown()
    {
        _woriedTimer = 0;
        _reacted = false;
        OnCalmDown.Invoke(_args);
    }
}
