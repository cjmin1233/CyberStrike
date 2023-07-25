using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyFadingCG : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool loop;
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
        cgText.text = text;
        animator.SetTrigger("FadeTrigger");
        animator.SetBool("Loop", loop);
    }
}
