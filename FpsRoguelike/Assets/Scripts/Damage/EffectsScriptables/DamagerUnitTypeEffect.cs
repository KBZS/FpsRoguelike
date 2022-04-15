using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffect/Damager/" + nameof(DamagerUnitTypeEffect))]
public class DamagerUnitTypeEffect : ScriptableObject
{
    public UnitType DamageableType => _damageableUnitType;
    public float DamageMultiplier => _damageMultiplier;

    [SerializeField] private UnitType _damageableUnitType;
    [SerializeField, Min(0)] float _damageMultiplier;
}
