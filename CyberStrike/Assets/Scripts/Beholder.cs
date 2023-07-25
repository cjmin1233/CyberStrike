using System;
using UnityEngine.Events;
using UnityEngine;

public class Beholder : Enemy
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    protected override void Awake()
    {
        base.Awake();

        enemyAnimatonEvent.onShoot.AddListener(Shoot);
    }

    private void Shoot()
    {
        var instance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        instance.GetComponent<EnemyBullet>().enemy = this;
    }
}
