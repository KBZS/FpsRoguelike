using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public UnitType Type => _unitType;
    [SerializeField] private UnitType _unitType;

    public DamagerDamageTypeEffect[] DamageTypeEffects;
    public DamagerUnitTypeEffect[] UnitTypeEffects;

    public void HitUnit(Damageable damageable, DamageType damageType, float damage)
    {
        float calculatedDamage = CalculateDamage(damageable, damageType, damage);
        damageable.Hit(this, damageType, calculatedDamage);
    }

    private float CalculateDamage(Damageable damageable, DamageType damageType, float damage)
    {
        float calculatedDamage = damage;

        if (DamageTypeEffects != null)
            foreach (DamagerDamageTypeEffect effect in DamageTypeEffects)
                if (effect.Type.Equals(damageType))
                    calculatedDamage += damage * effect.DamageMultiplier - damage;

        if (UnitTypeEffects != null)
            foreach (DamagerUnitTypeEffect effect in UnitTypeEffects)
                if (effect.DamageableType.Equals(damageable.Type))
                    calculatedDamage += damage * effect.DamageMultiplier - damage;

        return calculatedDamage;
    }
}
