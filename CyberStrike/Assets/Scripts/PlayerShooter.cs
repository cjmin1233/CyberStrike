using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using System;

public class PlayerShooter : MonoBehaviour
{
    public enum AimState
    {
        Sprinting,
        HipFire,
        Reloading
    }
    public enum WeaponType
    {
        AssaultRifle,
    }
    Animator playerAnimator;
    PlayerInput playerInput;
    RigBuilder rigBuilder;
    PlayerHealth playerHealth;

    Gun gun;
    [SerializeField] GameObject[] weapons;
    public WeaponType curWeaponType;
    [SerializeField] TwoBoneIKConstraint leftHandIK;

    RaycastHit aimHit;
    Ray camRay;
    [SerializeField] Transform viewPoint;
    Vector3 aimPoint;
    AimState aimState = AimState.HipFire;

    //private bool isReloading = false;
    //[SerializeField] private ReloadState reloadState
    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerHealth = GetComponent<PlayerHealth>();

        rigBuilder = GetComponentInChildren<RigBuilder>();

        // first weapon setting
        WeaponSetup();
    }
    private void Update()
    {
        if (playerHealth.isDead) return;
        UpdateAimState();
        UpdateAimPoint();
    }
    private void FixedUpdate()
    {
        if (playerHealth.isDead) return;

        if (aimState == AimState.HipFire)
        {
            if (playerInput.fire) Shoot();
            else if (playerInput.reload) Reload();
        }
        //if (playerInput.fire && aimState == AimState.HipFire) Shoot();
        //else if (playerInput.reload && aimState != AimState.Sprinting) Reload();
        //else playerAnimator.SetBool("IsFiring", false);
    }

    private void Reload()
    {
        if (gun.AttemptReload())
        {
            playerAnimator.SetBool("IsReloading", true);
            //aimState = AimState.Reloading;
            //isReloading = true;
        }
    }
    public void MagIn()
    {
        gun.MagIn();
    }
    public void CompleteReload()
    {
        print("장전 완료!");
        gun.CompleteReload();
        playerAnimator.SetBool("IsReloading", false);
    }
    public void WeaponSetup()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if (i == (int)curWeaponType)
            {
                gun = weapons[i].GetComponent<Gun>();
                gun.gameObject.SetActive(true);

                leftHandIK.data.target = gun.leftGrabPoint;
                rigBuilder.enabled = false;
                rigBuilder.enabled = true;
            }
            else weapons[i].SetActive(false);
        }
    }
    private void UpdateAimPoint()
    {
        //camRay = Camera.main.ViewportPointToRay(viewPoint);

        //if (Physics.Raycast(camRay, out aimHit, gun.fireDistance)) aimPoint = aimHit.point;
        if (Physics.Raycast(viewPoint.position, viewPoint.forward, out aimHit, gun.fireDistance)) aimPoint = aimHit.point;
        else aimPoint = viewPoint.position + viewPoint.forward * gun.fireDistance;
    }
    private void UpdateAimState()
    {
        if (playerAnimator.GetBool("IsSprinting"))
        {
            aimState = AimState.Sprinting;
            playerAnimator.SetBool("IsReloading", false);
        }
        else if (playerAnimator.GetBool("IsReloading")) aimState = AimState.Reloading;
        else aimState = AimState.HipFire;
    }
    private void Shoot()
    {
        if (gun.Fire(aimPoint)) playerAnimator.SetTrigger("Fire");
        else if (gun.gunState == Gun.GunState.Empty) Reload();
    }
    public void UpgradeWeapon()
    {
        if (gun.Upgrade()) print("Gun Upgraded");
    }
}
