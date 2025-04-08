using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Elf : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private float targetDistance;
    [SerializeField] float detectionDistance;
    [SerializeField] float detectionDistance2;
    [SerializeField] float fleeDistance;
    [SerializeField] float fleeRadius;
    private Rigidbody2D rb;
    [SerializeField] float dodgeForce;
    [SerializeField] TrailRenderer dashEffect;
    [SerializeField] float dodgeDuration = 0.2f;
    private bool shouldDodge;
    public Vector2 direction;
    private EnemySpriteControl sc;
    private float bombThrowTimer;
    [SerializeField] private GameObject bomb;
    [SerializeField] private float throwForce;
    [SerializeField] private float bombThrowCooldown;
    BowShooting shootingScript;
    [SerializeField] LayerMask raycastLayer;
    private bool canSeeTarget;

    [SerializeField] bool shouldFlee;
    [SerializeField] GameObject elf;
    [SerializeField] GameObject spawnEffect;
    private float ultTimer;
    private SpawnEnemies spawnScript;
    private float healthSave;
    public bool isClone=false;
    [SerializeField] GameObject elfCanvas;
    [SerializeField] Slider healthSlider;
    private float timeScale;
    private List<GameObject> clones=new List<GameObject>();

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        sc= GetComponent<EnemySpriteControl>();
        shootingScript=GetComponent<BowShooting>();
        spawnScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();

        ultTimer = Time.time + Random.Range(20, 35);
    }

    private void Update()
    {
        LerpHealth();

        if (ultTimer < Time.time && !isClone)
        {
            ultTimer = Time.time + 1000;
            shouldDodge = false;

            for (int i = 0; i < Random.Range(3, 7); i++)
            {
                Vector3 pos = spawnScript.RandomPoint(spawnScript.GetTilemapCenter(), 10);
                Instantiate(spawnEffect, pos, spawnEffect.transform.rotation);
                GameObject elfClone = Instantiate(elf, pos, elf.transform.rotation);
                GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive++;
                elfClone.GetComponent<EnemyHealth>().healhPoints = 10;
                elfClone.GetComponent<Elf>().shouldFlee = false;
                elfClone.GetComponent<Elf>().isClone = true;
                elfClone.GetComponent<Elf>().elfCanvas.SetActive(false);
                clones.Add(elfClone);

                Vector3 realPos = spawnScript.RandomPoint(spawnScript.GetTilemapCenter(), 10);
                Instantiate(spawnEffect, realPos, spawnEffect.transform.rotation);
                transform.position = realPos;
                healthSave = GetComponent<EnemyHealth>().healhPoints;
                shouldFlee = false;
            }
        }

        if (healthSave != GetComponent<EnemyHealth>().healhPoints && !shouldFlee && !isClone)
        {
            shouldDodge = true;
            shouldFlee = true;
            ultTimer = Time.time + Random.Range(20, 35);
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, 100, raycastLayer);
        if (hit.Length > 0)
        {
            if (hit[0].collider.gameObject.tag == "Player")
            {
                canSeeTarget = true;
            }
            else
            {

                canSeeTarget = false;
            }
        }

        targetDistance = Vector2.Distance(transform.position, target.position);
        if (targetDistance < detectionDistance && targetDistance>detectionDistance2 && shouldFlee)
        {
            agent.isStopped = false;

            Vector3 directionToPlayer = transform.position - target.position;

            Vector3 fleeDirection = directionToPlayer.normalized * fleeDistance;
            Vector3 randomDirection = Random.insideUnitCircle * fleeRadius;
            Vector3 fleeTarget = transform.position + fleeDirection + (Vector3)randomDirection;

            NavMeshHit randomPointHit;
            if (NavMesh.SamplePosition(fleeTarget, out randomPointHit, fleeRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(randomPointHit.position);
            }

            if (bombThrowTimer < Time.time && canSeeTarget)
            {
                Vector2 dir = (transform.position - target.position).normalized;
                GameObject bombClone = Instantiate(bomb, transform.position, transform.rotation);
                float distance = Vector2.Distance(transform.position, target.position);
                bombClone.GetComponent<Rigidbody2D>().AddForce(dir * throwForce * -1 * distance, ForceMode2D.Impulse);
                bombThrowTimer = Time.time + bombThrowCooldown;
            }
        }
        else if(detectionDistance2<targetDistance && shouldFlee)
        {
            Vector3 directionToPlayer = transform.position - target.position;

            Vector3 fleeDirection = directionToPlayer.normalized * fleeDistance;
            Vector3 randomDirection = Random.insideUnitCircle * fleeRadius;
            Vector3 fleeTarget = transform.position + fleeDirection + (Vector3)randomDirection;

            NavMeshHit randomPointHit;
            if (NavMesh.SamplePosition(fleeTarget, out randomPointHit, fleeRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(randomPointHit.position);
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            sc.isMoving = false;
            sc.shouldFacePlayer = true;

            if (canSeeTarget)
            {
                shootingScript.canShoot = true;
            }
            else
            {
                shootingScript.canShoot = false;
            }
        }
    }

    public IEnumerator StartDodge()
    {
        if (shouldDodge)
        {
            agent.SetDestination(transform.position);
            dashEffect.emitting = true;
            rb.velocity = direction * dodgeForce;
            yield return new WaitForSeconds(dodgeDuration);
            rb.velocity = Vector2.zero;
            dashEffect.emitting = false;
        }
    }

    void LerpHealth()
    {
        healthSlider.value = Mathf.Lerp(GetComponent<EnemyHealth>().healhPoints, healthSlider.value, Time.deltaTime * 10);
    }
}
