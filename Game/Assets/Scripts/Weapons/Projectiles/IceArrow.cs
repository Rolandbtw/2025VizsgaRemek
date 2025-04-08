using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : MonoBehaviour
{
    [SerializeField] GameObject iceCube;
    [SerializeField] GameObject iceDamageEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!collision.GetComponent<Runes>().inventoryIsOpened) 
            {
                collision.GetComponent<SpriteControl>().enabled = false;
                collision.GetComponent<Runes>().inventoryIsOpened = true;
                collision.GetComponent<Runes>().isFroozen = true;
                float speed = collision.GetComponent<PlayerMovement>().moveSpeed;
                collision.GetComponent<PlayerMovement>().moveSpeed = 0;
                GameObject iceCubeClone = Instantiate(iceCube, collision.transform.position, collision.transform.rotation);
                iceCubeClone.transform.parent = collision.transform;
                iceCubeClone.transform.localPosition += new Vector3(0, -0.05f, 0);
                iceCubeClone.GetComponent<IceCube>().movementSpeed = speed;
                Instantiate(iceDamageEffect, transform.position, iceDamageEffect.transform.rotation);
                Destroy(gameObject);
            }
        }
        if (collision.CompareTag("wall"))
        {
            Instantiate(iceDamageEffect, transform.position, iceDamageEffect.transform.rotation);
            Destroy(gameObject);
        }
    }
}
