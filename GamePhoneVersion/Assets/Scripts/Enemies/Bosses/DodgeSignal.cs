using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeSignal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && !collision.GetComponent<Bullets>().isIgnoringEnemy || collision.CompareTag("Weapon"))
        {
            Elf elfScript = GetComponentInParent<Elf>();
            int leftOrRight = Random.Range(0, 2);
            if (leftOrRight == 0)
            {
                elfScript.direction = collision.transform.up;
            }
            else
            {
                elfScript.direction = collision.transform.up * -1;
            }
            StopCoroutine(elfScript.StartDodge());
            StartCoroutine(elfScript.StartDodge());
        }
    }
}
