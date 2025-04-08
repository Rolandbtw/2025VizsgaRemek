using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SuicideGoblin : MonoBehaviour
{
    [Header("Explosion effects and objects")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject explosionCircle;

    [Header("Explosion settings")]
    [SerializeField] private float blowDistance;
    [SerializeField] private float blowOffset;

    private Rigidbody2D rb;
    private bool onlyCallOnce = true;
    private BombAnim sc;
    private NavMeshAgent agent;
    private Transform target;
    private float timer;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        timer = Time.time + 1f; ;
        sc=GetComponentInChildren<BombAnim>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) < blowDistance && onlyCallOnce && timer<Time.time)
        {
            GetComponent<EnemySpriteControl>().isMoving = false;
            StartCoroutine(Explosion());
            onlyCallOnce = false;
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    public IEnumerator Explosion()
    {
        agent.isStopped = true;
        soundScript.MakeSound("goblinSound", 0.5f);
        StartCoroutine(sc.ExplosionAnim(blowOffset/3));
        yield return new WaitForSeconds(blowOffset);
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Instantiate(blood, transform.position, transform.rotation);
        Instantiate(explosionCircle, transform.position, transform.rotation);
        GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive--;
        Destroy(gameObject);
    }
}
