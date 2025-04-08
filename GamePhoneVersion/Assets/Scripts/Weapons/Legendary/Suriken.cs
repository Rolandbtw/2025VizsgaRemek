using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Suriken : MonoBehaviour
{
    [SerializeField] Transform parentObject;
    [SerializeField] float duration = 5f;
    [SerializeField] int loops = 3;
    [SerializeField] float maxRadius = 5f;
    [SerializeField] float rotationSpeed;
    [SerializeField] float damage;
    [SerializeField] float knockBackForce;

    private Vector3 parentInitialPosition;
    [SerializeField] AudioClip surikenSound;
    [SerializeField] AudioMixerGroup auGroup;

    private void Start()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = auGroup;
        audioSource.clip = surikenSound;
        audioSource.volume = 0.1f;
        audioSource.loop = true;
        audioSource.Play();
        Destroy(tempAudio, duration);

        parentObject = GameObject.FindGameObjectWithTag("Player").transform;
        parentInitialPosition = parentObject.position;
        StartCoroutine(MoveInExpandingAndContractingCircles());
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * -1);
    }

    private IEnumerator MoveInExpandingAndContractingCircles()
    {
        float halfDuration = duration / 2f;
        float angle = 0f;

        float timeElapsed = 0f;
        while (timeElapsed < halfDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / halfDuration;

            float radius = Mathf.Lerp(0f, maxRadius, progress);

            angle += Time.deltaTime * (360f * loops / halfDuration);

            // Calculate position offset relative to the parent - got this from the net, idk how it works
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * radius;

            transform.position = parentObject.position + offset;

            yield return null;
        }

        timeElapsed = 0f;
        while (timeElapsed < halfDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / halfDuration;

            float radius = Mathf.Lerp(maxRadius, 0f, progress);

            angle += Time.deltaTime * (360f * loops / halfDuration);

            // Calculate position offset relative to the moving parent - got this from the net, idk how it works
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * radius;

            transform.position = parentObject.position + offset;

            yield return null;
        }
        transform.position = parentObject.position;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
            collision.gameObject.GetComponent<EnemyHealth>().KnockBackEnemy(knockBackForce, parentObject);
        }
    }
}
