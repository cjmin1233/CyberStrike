using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour, IItem
{
    [SerializeField] private ItemType itemType;

    public void Use(GameObject target)
    {
        var shooter = target.GetComponent<PlayerShooter>();
        if (shooter is null) return;
        shooter.UpgradeWeapon();
        ItemSpawner.Instance.Add2Pool((int)itemType, gameObject);
    }
}
