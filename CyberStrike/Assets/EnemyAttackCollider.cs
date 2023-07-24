using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] private float damage;
    private float damageMultiplier = 1f;
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    private void OnEnable()
    {
        damageMultiplier = enemy.damageMultiplier;
    }
    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            DamageMessage damageMessage;

            damageMessage.damager = enemy.transform.gameObject;
            damageMessage.damage = this.damage * damageMultiplier;
            damageMessage.hitPoint = other.ClosestPointOnBounds(transform.position);
            damageMessage.hitNormal = Vector3.zero;
            damageMessage.damageType = DamageType.Body;

            playerHealth.TakeDamage(damageMessage);
        }
    }
}
