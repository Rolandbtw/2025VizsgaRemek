using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NecromancerPathfinding : MonoBehaviour
{

    [Header("Flee settings (running away from player)")]
    [SerializeField] private float detectionRadius;
    [SerializeField] private float randomPointRange;
    [SerializeField] private float fleeDistance;

    [Header("Summoning enemies")]
    [SerializeField] private float summonCooldown;
    [SerializeField] private GameObject skeleton;
    [SerializeField] private GameObject zombie;
    [SerializeField] private GameObject summonEffect;

    private float timer;
    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody2D rb;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        rb.isKinematic = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        timer = Time.time + summonCooldown;
    }

    private void Update()
    {
        if (!GetComponent<EnemyHealth>().isDead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRadius)
            {
                agent.isStopped = false;

                Vector3 directionToPlayer = transform.position - player.position;

                Vector3 fleeDirection = directionToPlayer.normalized * fleeDistance;
                Vector3 randomDirection = Random.insideUnitCircle * randomPointRange;
                Vector3 fleeTarget = transform.position + fleeDirection + (Vector3)randomDirection;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(fleeTarget, out hit, randomPointRange, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
            else
            {
                agent.isStopped = true;
                if (timer < Time.time)
                {
                    SummonEnemies();
                    timer = Time.time + summonCooldown;
                }
            }
        }
    }

    void SummonEnemies()
    {
        soundScript.MakeSound("summonEnemiesSound", 0.5f);
        SpawnEnemies sc = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        int enemyCount = Random.Range(1, 5);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPoint = sc.RandomPoint(transform.position, 5);
            int enemyType = Random.Range(1, 6);
            if (enemyType == 1)
            {
                Instantiate(skeleton, spawnPoint, skeleton.transform.rotation);
            }
            else
            {
                Instantiate(zombie, spawnPoint, skeleton.transform.rotation);
            }
            Instantiate(summonEffect, spawnPoint, summonEffect.transform.rotation);
            GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive++;
        }
    }
}
