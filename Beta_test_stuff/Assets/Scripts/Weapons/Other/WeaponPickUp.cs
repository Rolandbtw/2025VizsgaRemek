using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    private TextMeshProUGUI pickUpSignal;
    private bool canPickUp;

    void Start()
    {
        pickUpSignal =GameObject.FindGameObjectWithTag("PickUpSignal").GetComponent<TextMeshProUGUI>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            pickUpSignal.text="Press "+ PlayerPrefs.GetString("Interact") +" to pick up " + transform.parent.name.Split('_')[0];
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp=false;
            pickUpSignal.text = "";
        }
    }

    public void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyBindings.interact) && !GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().usingWeapon)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().PickUp(transform.parent.name.Split('_')[0]);
            Destroy(transform.parent.gameObject);
        }
    }
}
