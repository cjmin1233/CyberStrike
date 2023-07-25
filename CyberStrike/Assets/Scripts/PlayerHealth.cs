using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerHealth : LivingEntity
{
    [SerializeField] private CinemachineVirtualCamera normalCam;
    [SerializeField] private CinemachineVirtualCamera deathCam;

    float value;
    private void Awake()
    {
        normalCam.Priority = 10;
        deathCam.Priority = 9;

        onDeath += OnPlayerDeath;
    }
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
        if (isDead) return;
        value = Mathf.Clamp01(health / maxHealth);
        UiManager.Instance.UpdatePlayerHealthBar(health,maxHealth);
    }
    private void OnPlayerDeath()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.applyRootMotion = true;
        animator.SetBool("IsDead", true);

        normalCam.Priority = 9;
        deathCam.Priority = 10;
    }
}
