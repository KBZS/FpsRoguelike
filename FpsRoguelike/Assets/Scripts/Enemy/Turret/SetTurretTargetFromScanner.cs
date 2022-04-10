using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTurretTargetFromScanner : MonoBehaviour
{
    [SerializeField] Scanner _scanner;
    [SerializeField] RotateTurretToTarget _turret;

    void Start()
    {
        _scanner.OnScannerViewEnter.AddListener(OnScannerViewEnter);
        _scanner.OnScannerViewExit.AddListener(OnScannerViewExit);
    }

    void OnScannerViewEnter(IScanerTarget target)
    {
        _turret.Target = target;
    }

    void OnScannerViewExit(IScanerTarget target)
    {
        _turret.Target = null;
    }

    private void OnDestroy()
    {
        _scanner.OnScannerViewEnter.RemoveListener(OnScannerViewEnter);
        _scanner.OnScannerViewExit.RemoveListener(OnScannerViewExit);
    }
}
