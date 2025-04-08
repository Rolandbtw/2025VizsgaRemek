using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceCube : MonoBehaviour
{
    public float duration;
    public float waitDuration;
    public Vector3 fullSize;
    public Vector3 smallSize;

    public float movementSpeed = 0;
    public bool isAimedAtPlayer = true;

    Sounds soundScript;

    IEnumerator Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        soundScript.MakeSound("iceSound", 0.5f);
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.localScale = Vector3.Lerp(smallSize, fullSize, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(waitDuration);

        timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.localScale = Vector3.Lerp(fullSize, smallSize, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (isAimedAtPlayer)
        {
            GetComponentInParent<PlayerMovement>().moveSpeed = movementSpeed;
            GetComponentInParent<Runes>().inventoryIsOpened = false;
            GetComponentInParent<SpriteControl>().enabled = true;
            GetComponentInParent<Runes>().isFroozen = false;
        }
        else
        {
            MonoBehaviour[] allScripts = GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour script in allScripts)
            {
                script.enabled = true;
            }
            GetComponentInParent<EnemyHealth>().isFroozen = false;

            if (!GetComponentInParent<EnemyHealth>().isDead)
            {
                GetComponentInParent<NavMeshAgent>().isStopped = false;
            }
        }
        Destroy(gameObject);
    }
}
