using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPickUp : MonoBehaviour
{
    private GameObject interactButton;
    private bool canPickUp;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        if (movementActions == null)
        {
            movementActions = new PlayerMovementInputActions();
        }
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        interactButton = GameObject.Find("InteractButton");
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            interactButton.GetComponent<Image>().enabled = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp=false;
            interactButton.GetComponent<Image>().enabled = false;
        }
    }

    public void Update()
    {
        if (canPickUp && movementActions.PlayerMap.Interact.triggered && !GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().usingWeapon)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().PickUp(transform.parent.name.Split('_')[0]);
            Destroy(transform.parent.gameObject);
        }
    }
}
