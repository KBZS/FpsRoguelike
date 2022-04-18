using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour, IScanerTarget
{
    public static Transform Instance;

    [SerializeField] CapsuleCollider _collider;

    public Vector3 Center => base.transform.position + Vector3.up * _collider.height / 2;

    public new Transform transform => base.transform;

    void Awake()
    {
        Instance = transform;
    }
}
