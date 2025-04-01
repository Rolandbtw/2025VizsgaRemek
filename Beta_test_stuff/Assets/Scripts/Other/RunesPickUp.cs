using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RunesPickUp : MonoBehaviour
{
    private TextMeshProUGUI pickUpText;
    private bool canPickUp=false;
    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        pickUpText =GameObject.FindGameObjectWithTag("PickUpSignal").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickUpText.text="Press "+ PlayerPrefs.GetString("Interact") +" to pick up "+name.Split('_')[0];
            canPickUp=true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickUpText.text = "";
            canPickUp=false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyBindings.interact) && canPickUp && !GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().inventoryIsOpened)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().PickUpRune(gameObject);
            soundScript.MakeSound("playerPickUpSound2", 0.5f);
            pickUpText.text = "";
        }
    }
}
