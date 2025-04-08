using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MudBoss : MonoBehaviour
{
    [SerializeField] GameObject zombie;
    [SerializeField] GameObject spawnEffect;
    [SerializeField] GameObject mudSpawnEffect;
    [SerializeField] float spawnCooldown;
    [SerializeField] GameObject mudProjectile;
    [SerializeField] float dashDistance;
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    private bool isDashing = false;
    private bool isSpawning = false;
    [SerializeField] TrailRenderer dashEffect;
    [SerializeField] float shootingCooldown;
    [SerializeField] float shootingForce;
    [SerializeField] float bulletCount;
    private float shootingTimer;
    private float spawnTimer;
    private NavMeshAgent agent;
    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] Slider healthSlider;
    private float timeScale;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        spawnTimer = Time.time + spawnCooldown;

    }

    private void Update()
    {
        LerpHealth();
        float distance = Vector2.Distance(transform.position, target.position);

        if (dashDistance > distance && !isDashing && !isSpawning)
        {
            PouncePlayer();
        }
        if (!isDashing && !isSpawning)
        {
            agent.SetDestination(target.position);
            if (shootingTimer < Time.time)
            {
                shootingTimer = Time.time + shootingCooldown;
                Shoot();
            }
        }
        if (spawnTimer < Time.time && !isSpawning)
        {
            isSpawning = true;
            agent.SetDestination(transform.position);
            GameObject effect = Instantiate(mudSpawnEffect, transform.position, Quaternion.identity);
            effect.transform.parent = transform;
            SpawnZombies();
            StartCoroutine(StopSpawnPhase());
        }
    }

    IEnumerator StopSpawnPhase()
    {
        yield return new WaitForSeconds(15);
        isSpawning = false;
        spawnTimer = Time.time + spawnCooldown;
    }

    void PouncePlayer()
    {
        isDashing = true;
        dashEffect.emitting = true;
        agent.enabled = false;
        rb.isKinematic = false;
        Vector2 dashDirection = (target.position - transform.position).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
        StartCoroutine(EndPounce());
    }

    IEnumerator EndPounce()
    {
        yield return new WaitForSeconds(dashDuration);
        if (!GetComponent<EnemyHealth>().isDead)
        {
            dashEffect.emitting = false;
            rb.velocity = Vector2.zero;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(rb.position, out hit, 10, NavMesh.AllAreas))
            {
                agent.enabled = true;
                agent.Warp(hit.position);
            }

            rb.isKinematic = true;
            isDashing = false;
        }
    }

    void Shoot()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360 / bulletCount) * Mathf.Deg2Rad;

            GameObject bulletClone = Instantiate(mudProjectile, transform.position, Quaternion.identity);
            bulletClone.GetComponent<Bullets>().damage = bulletClone.GetComponent<Bullets>().damage;

            Rigidbody2D bulletRb = bulletClone.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                Vector2 shootDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                bulletRb.AddForce(shootDirection * shootingForce, ForceMode2D.Impulse);
            }
        }
    }

    void SpawnZombies()
    {
        SpawnEnemies spawnScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        for (int i = 0; i < Random.Range(3, 8); i++)
        {
            Vector3 point = spawnScript.RandomPoint(transform.position, 10);
            Instantiate(zombie, point, zombie.transform.rotation);
            Instantiate(spawnEffect, point, spawnEffect.transform.rotation);
            spawnScript.currentEnemiesAlive++;
        }
    }

    void LerpHealth()
    {
        healthSlider.value = Mathf.Lerp(GetComponent<EnemyHealth>().healhPoints, healthSlider.value, Time.deltaTime * 10);
    }
}
