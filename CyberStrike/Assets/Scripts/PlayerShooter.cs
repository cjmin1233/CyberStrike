using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

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

    Gun gun;
    [SerializeField] GameObject[] weapons;
    public WeaponType weaponType;
    [SerializeField] TwoBoneIKConstraint leftHandIK;

    RaycastHit aimHit;
    Ray camRay;
    [SerializeField] Transform viewPoint;
    Vector3 aimPoint;
    AimState aimState;
    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();

        rigBuilder = GetComponentInChildren<RigBuilder>();

        // first weapon setting
        WeaponSetup();
    }
    private void Update()
    {
        UpdateAimState();
        UpdateAimPoint();
    }
    private void FixedUpdate()
    {
        if (playerInput.fire && aimState == AimState.HipFire) Shoot();
        else playerAnimator.SetBool("IsFiring", false);
    }
    public void WeaponSetup()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if (i == (int)weaponType)
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
    void UpdateAimPoint()
    {
        //camRay = Camera.main.ViewportPointToRay(viewPoint);

        //if (Physics.Raycast(camRay, out aimHit, gun.fireDistance)) aimPoint = aimHit.point;
        if (Physics.Raycast(viewPoint.position, viewPoint.forward, out aimHit, gun.fireDistance)) aimPoint = aimHit.point;
        else aimPoint = viewPoint.position + viewPoint.forward * gun.fireDistance;
    }
    void UpdateAimState()
    {
        if (playerAnimator.GetBool("IsSprinting")) aimState = AimState.Sprinting;
        else aimState = AimState.HipFire;
    }
    void Shoot()
    {
        playerAnimator.SetBool("IsFiring", true);
        gun.Fire(aimPoint);
    }
}
