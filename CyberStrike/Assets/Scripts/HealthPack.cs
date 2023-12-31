using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    [SerializeField] private ItemType itemType;
    [SerializeField] float amount;
    public void Use(GameObject target)
    {
        var livingEntity = target.GetComponent<LivingEntity>();
        if (livingEntity is null) return;
        livingEntity.RestoreHealth(amount);
        ItemSpawner.Instance.Add2Pool((int)itemType, gameObject);
    }
}
