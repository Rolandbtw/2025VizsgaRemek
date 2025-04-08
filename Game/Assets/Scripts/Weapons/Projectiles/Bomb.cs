using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject explosionCircle;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float blowOffset;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        StartCoroutine(Explosion());
    }

    public IEnumerator Explosion()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        StartCoroutine(GetComponent<BombAnim>().ExplosionAnim(blowOffset / 3));
        yield return new WaitForSeconds(blowOffset);
        soundScript.MakeSound("bombSound", 0.25f);
        Instantiate(explosionCircle, transform.position, transform.rotation);
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void KnockBackBomb(Transform point, float force)
    {
        Vector2 knockbackDirection = (transform.position - point.position).normalized;
        GetComponent<Rigidbody2D>().AddForce(knockbackDirection * force, ForceMode2D.Impulse);
    }
}
