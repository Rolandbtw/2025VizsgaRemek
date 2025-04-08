using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceBullet : MonoBehaviour
{
    [SerializeField] GameObject iceCube;
    [SerializeField] GameObject iceDamageEffect;
    [SerializeField] Color blue1;
    [SerializeField] Color blue2;
    [SerializeField] Color blue3;
    [SerializeField] Color blue4;
    [SerializeField] float damage;
    [SerializeField] float animSlowness;
    [SerializeField] float speedless;

    private bool oneTime = true;

    private Runes runes;
    private void Start()
    {
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        damage = damage * runes.playerRangedMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && oneTime)
        {
            oneTime = false;
            EnemyHealth health = collision.GetComponent<EnemyHealth>();
            switch (health.baseColor.r*255)
            {
                case 255:
                    health.baseColor = blue1;
                    collision.GetComponent<SpriteRenderer>().color = blue1;
                    break;
                case 200:
                    health.baseColor = blue2;
                    collision.GetComponent<SpriteRenderer>().color = blue2;
                    break;
                case 150:
                    health.baseColor = blue3;
                    collision.GetComponent<SpriteRenderer>().color = blue3;
                    break;
                case 100:
                    health.baseColor = blue4;
                    collision.GetComponent<SpriteRenderer>().color = blue4;
                    break;
            }

            collision.gameObject.GetComponent<EnemySpriteControl>().movingAnimSpeed *= animSlowness;
            collision.gameObject.GetComponent<EnemySpriteControl>().idleAnimSpeed *= animSlowness;
            health.DamageEnemy(damage);
            collision.gameObject.GetComponent<NavMeshAgent>().speed -= speedless;
            Destroy(gameObject);
        }
        if (collision.CompareTag("wall"))
        {
            Instantiate(iceDamageEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
