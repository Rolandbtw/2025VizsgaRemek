using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Color originalColor;
    private SpriteRenderer bloodRenderer;

    private void Start()
    {
        bloodRenderer = GetComponent<SpriteRenderer>();
        float randomZ = Random.Range(0f, 360f);
        float randomScale = Random.Range(0.1f, 0.2f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomZ);
        transform.localScale = new Vector3(randomScale, randomScale, 1);
        StartCoroutine(DryTheBlood());
    }

    IEnumerator DryTheBlood()
    {
        Color startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        Color endColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            bloodRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
