using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [Header("Damage settings")]
    [SerializeField] private float damage;
    [SerializeField] private bool hasKnockBack;
    [SerializeField] private float knockBackStrength;
    [SerializeField] private float attackCooldown;

    private float timer;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && timer < Time.time)
        {
            collision.transform.gameObject.GetComponent<PlayerHealth>().DamagePlayer(damage, true);
            if (hasKnockBack)
            {
                collision.transform.gameObject.GetComponent<PlayerHealth>().KnockBackPlayer(knockBackStrength, transform);
            }
            timer = Time.time + attackCooldown;
        }
    }
}
