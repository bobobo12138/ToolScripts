using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public static readonly Damage one = new Damage(1);
    public static readonly Damage max = new Damage(9999);

    private static Damage tempDamage;

    public int damage;
    public int impactForce;
    public Transform trans_DamageSource;

    public Damage(int damage, int impactForce = 0, Transform trans_DamageSource = null)
    {
        this.damage = damage;
        this.impactForce = impactForce;
        this.trans_DamageSource = trans_DamageSource;
    }

    public void SetValue(int damage, int impactForce = 0, Transform trans_DamageSource = null)
    {
        this.damage = damage;
        this.impactForce = impactForce;
        this.trans_DamageSource = trans_DamageSource;
    }
    /// <summary>
    /// 获得一个临时Damage对象以节约new
    /// </summary>
    /// <returns></returns>
    public static Damage GetTempDamage()
    {
        if (tempDamage == null) tempDamage = new Damage(0);
        return tempDamage;
    }

}
