using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class OgrePathfinding : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    private Transform target;
    private bool isJumping=false;
    [SerializeField] float attackDistance;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpDuration;
    [SerializeField] float fallDuration;
    private float timer;
    [SerializeField] float cooldown;
    [SerializeField] GameObject impactWave;
    [SerializeField] GameObject landingEffect;
    [SerializeField] Vector3 startSize;
    [SerializeField] Vector3 endSize;
    [SerializeField] float damage;
    [SerializeField] float knockBackForce;

    private NavMeshAgent agent;
    private Rigidbody2D rb;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
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
            if (Vector2.Distance(transform.position, target.position) < attackDistance && timer < Time.time && !isJumping)
            {
                StartCoroutine(Jumping());
                isJumping = true;
            }
            else if (!isJumping && timer < Time.time)
            {
                GetComponent<EnemySpriteControl>().isMoving = true;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.SetDestination(transform.position);
                GetComponent<EnemySpriteControl>().isMoving = false;
            }
        }
    }

    IEnumerator Jumping()
    {
        GetComponent<BoxCollider2D>().enabled = false;

        GameObject shadowClone=Instantiate(shadow, transform.position, shadow.transform.rotation);
        shadowClone.transform.localScale = startSize;

        Vector3 startPos= transform.position;
        Vector3 endPos= transform.position+=new Vector3(0, jumpHeight, 0);

        float timeElapsed = 0;
        while (timeElapsed < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / jumpDuration);
            shadowClone.transform.localScale=Vector3.Lerp(startSize, endSize, timeElapsed / jumpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;
        while (timeElapsed < fallDuration)
        {
            transform.position = Vector3.Lerp(endPos, startPos, timeElapsed / fallDuration);
            shadowClone.transform.localScale = Vector3.Lerp(endSize, startSize, timeElapsed / jumpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        soundScript.MakeSound("hammerSound", 0.5f);
        GameObject waveClone=Instantiate(impactWave, transform.position, impactWave.transform.rotation);
        waveClone.GetComponent<ImpactWave>().damage= damage;
        waveClone.GetComponent<ImpactWave>().knockBackForce = knockBackForce;
        waveClone.GetComponent<ImpactWave>().isEnemyGenerated = true;
        Instantiate(landingEffect, transform.position, landingEffect.transform.rotation);
        Destroy(shadowClone);

        timer = Time.time + cooldown;
        isJumping =false;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
