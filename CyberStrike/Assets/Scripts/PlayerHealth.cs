using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    float value;
    protected override void OnEnable()
    {
        isDead = false;
        maxHealth = originMaxHealth;
        health = maxHealth;
    }
    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<IItem>();
        if (item != null) item.Use(gameObject);
    }
    private void Update()
    {
        value = Mathf.Clamp01(health / maxHealth);
        UiManager.Instance.UpdatePlayerHealthBar(health,maxHealth);
    }
}
