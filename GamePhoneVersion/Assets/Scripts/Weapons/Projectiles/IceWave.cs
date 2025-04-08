using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceWave : MonoBehaviour
{
    [Header("Other stuff")]
    [SerializeField] GameObject iceCube;
    [Header("Animations varriables")]
    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 endScale;
    [SerializeField] Color originalColor;
    [SerializeField] float startAlpha;
    [SerializeField] float endAlpha;
    [SerializeField] float duration;
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
        if (collision.CompareTag("Enemy") && !collision.GetComponent<EnemyHealth>().isFroozen)
        {
            Vector3 iceCubePos=new Vector3(collision.transform.position.x, collision.transform.position.y+0.75f, collision.transform.position.z);
            GameObject iceCubeClone = Instantiate(iceCube, iceCubePos, collision.transform.rotation);
            iceCubeClone.transform.parent = collision.transform;
            iceCubeClone.GetComponent<IceCube>().isAimedAtPlayer = false;
            iceCubeClone.GetComponent<IceCube>().fullSize = new Vector3(4, 4, 1);
            iceCubeClone.GetComponent<IceCube>().waitDuration = 10;

            collision.GetComponent<EnemyHealth>().isFroozen = true;

            MonoBehaviour[] allScripts = collision.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in allScripts)
            {
                if (script.GetType() != typeof(EnemyHealth))
                {
                    script.enabled = false;
                }
            }
            if (collision.GetComponent<NavMeshAgent>().enabled == true)
            {
                collision.GetComponent<NavMeshAgent>().SetDestination(collision.transform.position);
            }
        }
    }
}
