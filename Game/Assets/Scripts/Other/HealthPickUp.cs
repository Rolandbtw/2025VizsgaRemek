using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    Sounds soundScript;
    [SerializeField] GameObject pickUpEffect;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            float playerHealth = other.gameObject.GetComponent<PlayerHealth>().healhPoints;
            if (playerHealth != 100)
            {
                if (playerHealth + 5 > 100)
                {
                    other.gameObject.GetComponent<PlayerHealth>().healhPoints = 100;
                }
                else
                {
                    other.gameObject.GetComponent<PlayerHealth>().healhPoints += 5;
                }
                soundScript.MakeSound("coinSound", 0.5f);
                Instantiate(pickUpEffect, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
