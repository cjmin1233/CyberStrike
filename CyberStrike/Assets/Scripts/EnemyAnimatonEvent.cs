using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimatonEvent : MonoBehaviour
{
    public UnityEvent onIdling;
    public UnityEvent onEnableAttack;
    public UnityEvent onDisableAttack;
    public UnityEvent onDieFinish;
    public void OnIdling() => onIdling.Invoke();
    public void OnEnableAttack() => onEnableAttack.Invoke();
    public void OnDisableAttack() => onDisableAttack.Invoke();
    public void OnDieFinish() => onDieFinish.Invoke();
}
