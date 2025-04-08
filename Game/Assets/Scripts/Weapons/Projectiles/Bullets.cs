using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject hitWallEffect;
    [SerializeField] GameObject parryEffect;
    [Header("Floats and bools to customize")]
    public float damage;
    [SerializeField] float parryForce;
    public bool isIgnoringEnemy = false;

    private Rigidbody2D rb;

    Sounds soundScript;

    private void Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isIgnoringEnemy)
        {
            collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
            Destroy(gameObject);
        }
        if (collision.CompareTag("wall"))
        {
            Instantiate(hitWallEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        if(collision.CompareTag("Player") && isIgnoringEnemy)
        {
            collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(damage, false);
            Destroy(gameObject);
        }
        if(collision.CompareTag("Weapon") && collision.GetComponent<BasicSlicing>().isSlicing || collision.CompareTag("Suriken"))
        {
            soundScript.MakeSound("parrySound", 0.5f);
            Instantiate(parryEffect, transform.position, transform.rotation);
            GetComponent<SpriteRenderer>().flipX = true;
            damage *= 5;
            rb.velocity = -rb.velocity;
            isIgnoringEnemy = false;
        }
    }
}
