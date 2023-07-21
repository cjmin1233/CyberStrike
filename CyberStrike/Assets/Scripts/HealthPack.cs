using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    [SerializeField] float amount;
    public void Use(GameObject target)
    {
        target.GetComponent<LivingEntity>().RestoreHealth(amount);
        Destroy(gameObject);
    }
}
