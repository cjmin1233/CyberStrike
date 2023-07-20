using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    public UnityEvent onReloadComplete;
    public UnityEvent onMagIn;

    public void OnReloadComplete()
    {
        onReloadComplete.Invoke();
    }
    public void OnMagIn()
    {
        onMagIn.Invoke();
    }
}
