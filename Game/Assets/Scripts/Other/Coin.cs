using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    Sounds soundScript;
    [SerializeField] GameObject coinPickUpEffect;
    private Image coinPicture;

    private void Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        coinPicture=GameObject.FindGameObjectWithTag("CoinPicture").GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(coinPickUpEffect, transform.position, transform.rotation);
            soundScript.MakeSound("coinSound", 0.5f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Wallet>().coins+=Random.Range(1, 6);
            StartCoroutine(MoveToPlace());
        }
    }

    IEnumerator MoveToPlace()
    {
        Vector3 initialPos=transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            transform.position = Vector3.Lerp(initialPos, Camera.main.ScreenToWorldPoint(coinPicture.transform.position), elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
