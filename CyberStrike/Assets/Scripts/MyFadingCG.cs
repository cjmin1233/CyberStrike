using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyFadingCG : MonoBehaviour
{
    [SerializeField] private float speed;
    private Animator animator;
    private TextMeshProUGUI cgText;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        cgText = GetComponentInChildren<TextMeshProUGUI>();

        animator.SetFloat("Speed", speed);
    }
    public void TriggerFadingCG(string text)
    {
        if (cgText is null) cgText = GetComponentInChildren<TextMeshProUGUI>();

        if (cgText is null)
        {
            print("����!");
            return;
        }
        cgText.text = text;
        animator.SetTrigger("FadeTrigger");
    }
}
