using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public UnitType Type => _unitType;
    [SerializeField] private UnitType _unitType;
    [SerializeField] private Health _health;

    public DamageableDamageTypeEffect[] DamageTypeEffects;
    public DamageableUnitTypeEffect[] UnitTypeEffects;

    public float Hit(Damager damager, DamageType damageType, float damage)
    {
        float calculatedDamage = CalculateDamage(damager, damageType, damage);
        _health.Hit(calculatedDamage);
        return calculatedDamage;
    }

    private float CalculateDamage(Damager damager, DamageType damageType, float damage)
    {
        float calculatedDamage = damage;

        if (DamageTypeEffects != null)
            foreach (DamageableDamageTypeEffect effect in DamageTypeEffects)
                if (effect.Type.Equals(damageType))
                    calculatedDamage += damage * effect.DamageMultiplier - damage;

        if (UnitTypeEffects != null)
            foreach (DamageableUnitTypeEffect effect in UnitTypeEffects)
                if (effect.DamagerType.Equals(damager.Type))
                    calculatedDamage += damage * effect.DamageMultiplier - damage;

        return calculatedDamage;
    }
}