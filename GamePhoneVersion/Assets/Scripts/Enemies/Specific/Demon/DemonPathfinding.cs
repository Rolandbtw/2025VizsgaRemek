using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class DemonPathfinding : MonoBehaviour
{
    [Header("Pounce effect")]
    [SerializeField] TrailRenderer pounceEffect;
    [Header("Pounce settings (dash towrads player)")]
    [SerializeField] float pounceDistance;
    [SerializeField] float pounceDuration;
    [SerializeField] float dashForce;
    [Header("Wall hitting settings and object")]
    [SerializeField] float collisionRadius;
    [SerializeField] GameObject bloodEffect;

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody2D rb;
    private bool isPouncing = false;
    [SerializeField] LayerMask raycastLayer;

    private float startingCooldownTimer;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        pounceEffect.emitting = false;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        startingCooldownTimer = Time.time + 3f;
    }

    private void Update()
    {
        if (!GetComponent<EnemyHealth>().isDead)
        {
            if (Vector2.Distance(transform.position, target.position) < pounceDistance && !isPouncing && startingCooldownTimer < Time.time)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, 100, raycastLayer);

                if (hit.Length > 0)
                {
                    if (hit[0].collider.gameObject.tag == "Player")
                    {
                        PouncePlayer();
                    }
                }
            }
            else if (!isPouncing)
            {
                agent.SetDestination(target.position);
            }
        }
    }

    void PouncePlayer()
    {
        soundScript.MakeSound("demonDashSound", 0.5f);
        pounceEffect.emitting = true;
        isPouncing = true;
        agent.enabled = false;
        rb.isKinematic = false;
        Vector2 dashDirection = (target.position - transform.position).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
        StartCoroutine(EndPounce());
    }

    IEnumerator EndPounce()
    {
        yield return new WaitForSeconds(pounceDuration);
        pounceEffect.emitting = false;
        rb.velocity = Vector2.zero;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rb.position, out hit, 10, NavMesh.AllAreas) && !GetComponent<EnemyHealth>().isDead)
        {
            agent.enabled = true;
            agent.Warp(hit.position);
            rb.isKinematic = true;
            isPouncing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPouncing && collision.CompareTag("wall"))
        {
            GetComponent<EnemyHealth>().DamageEnemy(10);
            Instantiate(bloodEffect, transform.position, bloodEffect.transform.rotation);
        }
    }
}
