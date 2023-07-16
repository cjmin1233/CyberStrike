using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamagable
{
    public float maxHealth;
    public float health { get; protected set; }
    public bool isDead { get; protected set; }
    public event Action onDeath;

    protected virtual void OnEnable()
    {
        isDead = false;
        health = maxHealth;
    }
    public virtual void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject || isDead) return;

        health = Mathf.Clamp(health - damageMessage.damage, 0f, maxHealth);

        if (health <= 0f) Die();
    }
    public virtual void RestoreHealth(float amount)
    {
        if (isDead) return;

        health = Mathf.Clamp(health + amount, 0f, maxHealth);
    }
    public virtual void Die()
    {
        if (onDeath != null) onDeath();

        isDead = true;
    }
}
