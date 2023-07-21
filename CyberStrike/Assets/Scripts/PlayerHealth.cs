using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<IItem>();
        if (item != null) item.Use(gameObject);
    }
}
