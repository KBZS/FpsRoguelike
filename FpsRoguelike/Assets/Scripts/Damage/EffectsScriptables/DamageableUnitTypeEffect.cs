using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffect/Damageable/" + nameof(DamageableUnitTypeEffect))]
public class DamageableUnitTypeEffect : ScriptableObject
{
    public UnitType DamagerType => _damagerUnitType;
    public float DamageMultiplier => _damageMultiplier;

    [SerializeField] private UnitType _damagerUnitType;
    [SerializeField, Min(0)] float _damageMultiplier;
}
