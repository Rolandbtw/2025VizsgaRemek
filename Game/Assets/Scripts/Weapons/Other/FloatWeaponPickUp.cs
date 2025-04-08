using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FloatWeaponPickUp : MonoBehaviour
{
    [SerializeField] float moveSpeed=1;
    [SerializeField] float moveDistance=0.005f;
    [SerializeField] float shadowShrinkValue;
    private Rigidbody2D rb;
    private bool canFloat;
    private Vector3 startPosition;
    private Vector3 childStartPosition;
    private float t = 0;
    private Transform shadow;
    private Runes runeSciprt;

    private void Start()
    {
        runeSciprt=GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        rb = GetComponent<Rigidbody2D>();
        shadow=transform.GetChild(0);
    }

    private void Update()
    {
        if (!runeSciprt.inventoryIsOpened)
        {
            if (rb.velocity == new Vector2(0, 0))
            {
                canFloat = true;
                startPosition = transform.position;
                childStartPosition = shadow.position;

            }
            else
            {
                canFloat = false;
            }

            if (canFloat)
            {
                float newY = Mathf.Lerp(startPosition.y - moveDistance, startPosition.y + moveDistance, Mathf.PingPong(t, 1));
                float newChildSizeX = Mathf.Lerp(shadow.localScale.x + shadowShrinkValue, shadow.localScale.x - shadowShrinkValue, Mathf.PingPong(t, 1));
                float newChildSizeY = Mathf.Lerp(shadow.localScale.y + shadowShrinkValue / 2, shadow.localScale.y - shadowShrinkValue / 2, Mathf.PingPong(t, 1));
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                shadow.position = childStartPosition;
                shadow.localScale = new Vector3(newChildSizeX, newChildSizeY, shadow.localScale.z);
                t += Time.deltaTime * moveSpeed;
            }
        }
    }
}
