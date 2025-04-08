using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    [SerializeField] Sprite[] columnStages;
    [SerializeField] float durabilityPoints = 0;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject piecesParent;
    [SerializeField] float fallApartForce;
    [SerializeField] GameObject flame;
    [SerializeField] GameObject flameDestroyEffect;
    [SerializeField] Transform center;
    [SerializeField] Transform halfPoint;

    private float timer;
    private float timer2;
    [SerializeField] SpriteRenderer columnRenderer;
    private Sounds soundScript;
    private float direction; // 1 right, -1 left
    private bool oneTime = true;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
    }

    private void Update()
    {
        switch (durabilityPoints)
        {
            case 5:
                if (oneTime)
                {
                    FallApart();
                    Vector3 effectPos = new Vector3(flame.transform.position.x, flame.transform.position.y - 0.4f, flame.transform.position.z);
                    Instantiate(flameDestroyEffect, effectPos, flame.transform.rotation);
                    Destroy(flame);
                    oneTime = false;
                }
                break;
            case 4:
                columnRenderer.sprite = columnStages[3];
                break;
            case 3:
                columnRenderer.sprite = columnStages[2];
                break;
            case 2:
                columnRenderer.sprite = columnStages[1];
                break;
            case 1:
                columnRenderer.sprite = columnStages[0];
                break;
        }
    }

    void FallApart()
    {
        columnRenderer.enabled = false;
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        piecesParent.SetActive(true);

        foreach (Transform piece in piecesParent.transform)
        {
            float randomAngle = 0;
            if (direction == 1)
            {
                randomAngle = Random.Range(50f, 70f);
            }
            else
            {
                randomAngle = Random.Range(-50f, -70f);
            }
            Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * transform.right * direction;
            piece.GetComponent<Rigidbody2D>().AddForce(randomDirection * fallApartForce, ForceMode2D.Impulse);
            float randomTorque = Random.Range(-0.5f, 0.5f);
            piece.GetComponent<Rigidbody2D>().AddTorque(randomTorque, ForceMode2D.Impulse);
        }
        StartCoroutine(ScaleDownPieces());
        Destroy(gameObject, 10);
    }

    private IEnumerator ScaleDownPieces()
    {
        Vector3 initialScale = new Vector3(1, 1, 1);
        float elapsedTime = 0f;

        while (elapsedTime < 10)
        {
            foreach (Transform piece in piecesParent.transform)
            {
                piece.localScale = Vector3.Lerp(initialScale, new Vector3(0.01f, 0.01f, 0.01f), elapsedTime / 10);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && timer2 < Time.time || collision.CompareTag("Explosion") && timer2 < Time.time)
        {
            if (collision.CompareTag("Bullet"))
            {
                Destroy(collision.gameObject);
            }
            Vector3 effectPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Instantiate(damageEffect, effectPos, transform.rotation);
            durabilityPoints++;
            timer2 = Time.time + 0.11f; ;
            soundScript.MakeSound("columnBreak", 0.5f);

            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
        }
        if (collision.CompareTag("Player") && collision.gameObject.transform.position.y > halfPoint.transform.position.y)
        {
            columnRenderer.sortingOrder = 10;
            Color currentColor = columnRenderer.color;
            currentColor.a = 0.5f;
            columnRenderer.color = currentColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            columnRenderer.sortingOrder = 3;
            Color currentColor = columnRenderer.color;
            currentColor.a = 1;
            columnRenderer.color = currentColor;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") && collision.gameObject.GetComponent<BasicSlicing>().isSlicing && timer < Time.time)
        {
            durabilityPoints++;
            Vector3 effectPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Instantiate(damageEffect, effectPos, transform.rotation);
            timer = Time.time + 0.25f;
            soundScript.MakeSound("columnBreak", 0.5f);

            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
        }
    }
}
