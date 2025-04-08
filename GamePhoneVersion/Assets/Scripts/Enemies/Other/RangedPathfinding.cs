using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class RangedPathfinding : MonoBehaviour
{
    [Header("Flee settings (running away from player)")]
    [SerializeField] private float stopDistance;
    [SerializeField] private float randomPointRange;
    [SerializeField] private float fleeDistance;

    [Header("Raycast layers (player and the tilemap layer")]
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField] Transform raycastPoint;

    [Header("Skeleton or Wizard")]
    [SerializeField] bool isSkeleton;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody2D rb;
    private bool isFleeing;
    private BowShooting bowScript;
    private StaffShooting staffScript;

    private void Start()
    {
        if (isSkeleton)
        {
            bowScript = GetComponent<BowShooting>();
        }
        else
        {
            staffScript = GetComponent<StaffShooting>();
        }
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
            Vector2 direction = (player.transform.position - transform.position).normalized;
            RaycastHit2D[] hit = Physics2D.RaycastAll(raycastPoint.position, direction, 100, raycastLayer);

            if (hit.Length > 0)
            {
                if (hit[0].collider.gameObject.tag == "Player")
                {
                    if (isSkeleton)
                    {
                        bowScript.canShoot = true;
                    }
                    else
                    {
                        staffScript.canShoot = true;
                    }
                    if (!isFleeing)
                    {
                        agent.SetDestination(transform.position);
                        GetComponent<EnemySpriteControl>().isMoving = false;
                    }
                }
                else
                {
                    if (isSkeleton)
                    {
                        bowScript.canShoot = false;
                    }
                    else
                    {
                        staffScript.canShoot = false;
                    }
                    if (!isFleeing)
                    {
                        agent.SetDestination(player.position);
                        GetComponent<EnemySpriteControl>().isMoving = true;
                    }
                }
            }
            if (isFleeing)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    Vector3 directionToPlayer = transform.position - player.position;
                    Vector3 fleeDirection = directionToPlayer.normalized * fleeDistance;
                    Vector3 randomDirection = Random.insideUnitCircle * randomPointRange;
                    Vector3 fleeTarget = transform.position + fleeDirection + (Vector3)randomDirection;

                    NavMeshHit hit2;
                    if (NavMesh.SamplePosition(fleeTarget, out hit2, randomPointRange, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit2.position);
                    }
                }
            }
        }
    }

    public void FleeForYourLife()
    {
        if (!isSkeleton)
        {
            staffScript.canShoot = false;
        }

        isFleeing = true;
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
}
