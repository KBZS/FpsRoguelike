using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffect/Damager/" + nameof(DamagerDamageTypeEffect))]
public class DamagerDamageTypeEffect : ScriptableObject
{
    public DamageType Type => _damageType;
    public float DamageMultiplier => _damageMultiplier;

    [SerializeField] private DamageType _damageType;
    [SerializeField, Min(0)] float _damageMultiplier;
}
