using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactWave : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject fireEffect;
    [Header("Floats to customize")]
    public float damage;
    public float knockBackForce;
    [Header("Animations varriables")]
    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 endScale;
    [SerializeField] Color originalColor;
    [SerializeField] float startAlpha;
    [SerializeField] float endAlpha;
    [SerializeField] float duration;
    [Header("Bools")]
    public bool isFireWave=false;
    public bool isEnemyGenerated;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        StartCoroutine(ScaleObject());
    }
    private IEnumerator ScaleObject()
    {
        Color startColor = new Color(originalColor.r, originalColor.g, originalColor.b, startAlpha);
        Color endColor = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        transform.localScale = endScale;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isEnemyGenerated)
        {
            if (isFireWave)
            {
                GameObject fire=Instantiate(fireEffect, transform.position, transform.rotation);
                fire.transform.SetParent(collision.gameObject.transform);
                fire.transform.localPosition = new Vector3(0, 0.115f, 0);
                fire.transform.eulerAngles = new Vector3(-90, 0, 0);
                fire.transform.localScale = new Vector3(1, 1, 1);

                collision.gameObject.GetComponent<EnemyHealth>().FireDamageEnemy(damage);
            }
            else 
            {
                collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
                collision.gameObject.GetComponent<EnemyHealth>().KnockBackEnemy(knockBackForce, transform);
            }
        }
        if (collision.CompareTag("Bomb"))
        {
            collision.gameObject.GetComponent<Bomb>().KnockBackBomb(transform, knockBackForce);
        }
        if(collision.CompareTag("Player") && isEnemyGenerated)
        {
            collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(damage, true);
            collision.gameObject.GetComponent<PlayerHealth>().KnockBackPlayer(knockBackForce, transform);
        }
    }
}
