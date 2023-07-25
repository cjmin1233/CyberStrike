using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HitBody : MonoBehaviour, IDamagable
{
    [SerializeField] private DamageType damageType;
    [SerializeField] private LivingEntity originBody;
    public void TakeDamage(DamageMessage damageMessage)
    {
        damageMessage.damageType = this.damageType;
        if (this.damageType == DamageType.Head) damageMessage.damage *= 2f;
        originBody.TakeDamage(damageMessage);
    }
}
