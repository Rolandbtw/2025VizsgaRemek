using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LittleGuy : MonoBehaviour
{
    Transform target;
    Rigidbody2D rb;
    private float runSpeed;
    Vector2 moveDirection;
    [SerializeField] GameObject impactWave;
    [SerializeField] GameObject demon;
    [SerializeField] GameObject spawnEffect;
    [SerializeField] float impactWaveDamage;
    [SerializeField] float impactWaveKnockBack;
    EnemySpriteControl spriteScript;
    bool canRun = true;
    bool hasBeenHit = false;
    private float speedSave;

    [SerializeField] Slider healthSlider;
    SpawnEnemies spawnScript;
    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        speedSave = runSpeed;
        spriteScript = GetComponent<EnemySpriteControl>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        spawnScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        rb = GetComponent<Rigidbody2D>();
        SetTargetDirection();
    }

    void Update()
    {
        LerpHealth();
        if (canRun)
        {
            rb.velocity = moveDirection * runSpeed;
        }
    }

    private void SetTargetDirection()
    {
        moveDirection = (target.position - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            if (hasBeenHit || runSpeed > 30)
            {
                canRun = false;
                StartCoroutine(RunCooldown());
            }
            soundScript.MakeSound("hammerSound", 0.5f);
            GameObject waveClone = Instantiate(impactWave, transform.position, impactWave.transform.rotation);
            waveClone.GetComponent<ImpactWave>().damage = impactWaveDamage;
            waveClone.GetComponent<ImpactWave>().knockBackForce = impactWaveKnockBack;
            waveClone.GetComponent<ImpactWave>().isEnemyGenerated = true;
            moveDirection = (target.position - transform.position).normalized;
            runSpeed += 2.5f;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(35, true);
            collision.gameObject.GetComponent<PlayerHealth>().KnockBackPlayer(15, transform);
            moveDirection = moveDirection * -1;
        }
        if (collision.gameObject.CompareTag("Weapon") && collision.gameObject.GetComponent<BasicSlicing>().isSlicing && !hasBeenHit)
        {
            rb.velocity = Vector3.zero;
            moveDirection = moveDirection * -1;
            runSpeed = runSpeed * 2;
            hasBeenHit = true;
        }
    }

    IEnumerator RunCooldown()
    {
        for (int i = 0; i < Random.Range(2, 6); i++)
        {
            Vector2 point = spawnScript.RandomPoint(transform.position, 10);
            Instantiate(demon, point, demon.transform.rotation);
            Instantiate(spawnEffect, point, spawnEffect.transform.rotation);
            GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive++;
        }

        rb.velocity = Vector2.zero;
        spriteScript.isMoving = false;
        yield return new WaitForSeconds(Random.Range(3, 10));
        soundScript.MakeSound("hammerSound", 0.5f);
        GameObject waveClone = Instantiate(impactWave, transform.position, impactWave.transform.rotation);
        waveClone.GetComponent<ImpactWave>().damage = impactWaveDamage;
        waveClone.GetComponent<ImpactWave>().knockBackForce = impactWaveKnockBack;
        waveClone.GetComponent<ImpactWave>().isEnemyGenerated = true;
        yield return new WaitForSeconds(1);
        hasBeenHit = false;
        runSpeed = speedSave;
        canRun = true;
        spriteScript.isMoving = true;
    }

    void LerpHealth()
    {
        healthSlider.value = Mathf.Lerp(GetComponent<EnemyHealth>().healhPoints, healthSlider.value, Time.deltaTime * 10);
    }
}
