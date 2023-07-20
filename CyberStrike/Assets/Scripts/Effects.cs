using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    [SerializeField] EffectManager.EffectType effectType;
    private void OnDisable()
    {
        EffectManager.instance.Add2Pool((int)effectType, gameObject);
    }
}
