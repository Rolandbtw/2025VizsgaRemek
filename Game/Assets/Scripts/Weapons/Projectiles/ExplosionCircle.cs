using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCircle : MonoBehaviour
{
    [SerializeField] float knockBackForce;
    [SerializeField] float damage;
    private void Start()
    {
        StartCoroutine(Disapear());
    }

    IEnumerator Disapear()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().KnockBackPlayer(knockBackForce, transform);
            collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(damage, false);
        }
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHealth>().KnockBackEnemy(knockBackForce, transform);
            collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
        }
        if (collision.CompareTag("Bomb"))
        {
            collision.gameObject.GetComponent<Bomb>().KnockBackBomb(transform, knockBackForce);
        }
    }
}
