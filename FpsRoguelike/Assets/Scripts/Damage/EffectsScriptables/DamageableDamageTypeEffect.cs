using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffect/Damageable/" + nameof(DamageableDamageTypeEffect))]
public class DamageableDamageTypeEffect : ScriptableObject
{
    public DamageType Type => _damageType;
    public float DamageMultiplier => _damageMultiplier;

    [SerializeField] private DamageType _damageType;
    [SerializeField, Min(0)] float _damageMultiplier;
}
