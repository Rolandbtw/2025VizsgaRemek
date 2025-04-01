using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrowing : MonoBehaviour
{
    [Header("Projectile object")]
    [SerializeField] private GameObject bomb;
    [Header("Throw settings")]
    [SerializeField] private float throwCooldown;
    [SerializeField] private float throwForce;
    [Header("Needs to be accessed")]
    public bool canShoot;

    private float timer;
    private Transform target;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        timer=Time.time+1f;
    }

    private void Update()
    {
        if (canShoot && timer<Time.time) 
        {
            soundScript.MakeSound("bombThrowSound", 0.5f);
            Vector2 dir = (transform.position - target.position).normalized;
            GameObject bombClone=Instantiate(bomb, transform.position, transform.rotation);
            float distance = Vector2.Distance(transform.position, target.position);
            bombClone.GetComponent<Rigidbody2D>().AddForce(dir*throwForce*-1*distance, ForceMode2D.Impulse);
            timer = Time.time + throwCooldown;
        }
    }
}
