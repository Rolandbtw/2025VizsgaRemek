using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class PumpkinPathfinding : MonoBehaviour
{
    [Header("Flee settings (running away from player)")]
    [SerializeField] private float stopDistance;
    [SerializeField] private float randomPointRange;
    [SerializeField] private float fleeDistance;

    [Header("Raycast layers (player and the tilemap layer")]
    [SerializeField] private LayerMask raycastLayer;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody2D rb;
    private bool isFleeing;
    private Vector3 fleeingPoint;
    private BombThrowing sc;

    private void Start()
    {
        sc = GetComponent<BombThrowing>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (!GetComponent<EnemyHealth>().isDead)
        {
            if (!isFleeing)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, 100, raycastLayer);

                if (hit.Length > 0)
                {
                    if (hit[0].collider.gameObject.tag == "Player")
                    {
                        sc.canShoot = true;
                        agent.isStopped = true;
                        GetComponent<EnemySpriteControl>().isMoving = false;
                    }
                    else
                    {
                        sc.canShoot = false;
                        agent.isStopped = false;
                        agent.SetDestination(player.position);
                        GetComponent<EnemySpriteControl>().isMoving = true;
                    }
                }
            }
            else
            {
                if (FindFleeTarget(out fleeingPoint))
                {
                    agent.SetDestination(fleeingPoint);
                }
            }
        }
    }

    public void FleeForYourLife()
    {
        sc.canShoot = false;
        isFleeing = true;
        agent.isStopped = false;
        agent.speed = agent.speed * 2;
        GetComponent<EnemySpriteControl>().isMoving = true;
        StartCoroutine(StopFleeing());
    }

    IEnumerator StopFleeing()
    {
        yield return new WaitForSeconds(Random.Range(3, 7));
        agent.speed = agent.speed / 2;
        isFleeing = false;
    }

    bool FindFleeTarget(out Vector3 target)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * randomPointRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, randomPointRange, 1))
            {
                if (Vector3.Distance(hit.position, player.position) > fleeDistance)
                {
                    target = hit.position;
                    return true;
                }
            }
        }
        target = Vector3.zero;
        return false;
    }
}

