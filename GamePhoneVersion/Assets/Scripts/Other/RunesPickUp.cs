using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunesPickUp : MonoBehaviour
{
    private GameObject interactButton;
    private bool canPickUp=false;
    Sounds soundScript;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        movementActions = new PlayerMovementInputActions();
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        interactButton = GameObject.FindGameObjectWithTag("Interact");
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(interactButton != null)
            {
                interactButton.GetComponent<Image>().enabled = true;
                canPickUp = true;
            }
            else
            {
                interactButton = GameObject.Find("InteractButton");
                interactButton.GetComponent<Image>().enabled = true;
                canPickUp = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactButton.GetComponent<Image>().enabled = false;
            canPickUp =false;
        }
    }

    private void Update()
    {
        if (movementActions.PlayerMap.Interact.triggered && canPickUp && !GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().inventoryIsOpened)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().PickUpRune(gameObject);
            soundScript.MakeSound("playerPickUpSound2", 0.5f);
            interactButton.GetComponent<Image>().enabled = false;
        }
    }
}
