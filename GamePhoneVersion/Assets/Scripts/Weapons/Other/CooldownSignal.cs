using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownSignal : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        animator.Play("CooldownAnim", 0, 0f);
        animator.speed = 0f;
    }

    private void OnEnable()
    {
        animator.Play("CooldownAnim", 0, 0f);
        animator.speed = 100f;
    }

    public void PlayCooldownAnim(float duration)
    {
        animator.Play("CooldownAnim", 0, 0f);

        animator.speed = 1f;

        StartCoroutine(SetCooldownAnimSpeed(duration));
    }

    private IEnumerator SetCooldownAnimSpeed(float duration)
    {
        duration += 0.25f;
        yield return null;

        AnimatorStateInfo animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = animationStateInfo.length;

        if (clipLength > 0)
        {
            float newSpeed = clipLength / duration;
            animator.speed = newSpeed;
        }
    }
}
