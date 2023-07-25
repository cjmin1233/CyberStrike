using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [HideInInspector] public Enemy enemy;
    private Rigidbody rb;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 10f;
        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            DamageMessage damageMessage;

            damageMessage.damager = enemy.transform.gameObject;
            damageMessage.damage = this.damage * enemy.damageMultiplier;
            damageMessage.hitPoint = other.ClosestPointOnBounds(transform.position);
            damageMessage.hitNormal = Vector3.zero;
            damageMessage.damageType = DamageType.Body;

            playerHealth.TakeDamage(damageMessage);
            Destroy(gameObject);
        }
    }
}
