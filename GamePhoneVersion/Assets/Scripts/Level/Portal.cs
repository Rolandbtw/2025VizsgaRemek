using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Portal : MonoBehaviour
{
    private bool oneTime = true;
    Sounds soundScript;
    SpawnEnemies spawnEnemiesScript;
    private bool isInPortal = false;
    Transform player;
    private GameObject interactButton;

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
        interactButton = GameObject.Find("InteractButton");
        spawnEnemiesScript= GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        soundScript =GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        soundScript.MakeSound("portalSpawnSound", 0.5f);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player.position.x > transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void Update()
    {
        if(movementActions.PlayerMap.Interact.triggered && oneTime && isInPortal && !player.GetComponent<Inventory>().usingWeapon)
        {
            interactButton.GetComponent<Image>().enabled = false;
            soundScript.MakeSound("portalSound", 0.5f);
            spawnEnemiesScript.NewRoom(gameObject.transform.GetChild(0).transform.position);
            Destroy(gameObject, 1.4f);
            oneTime = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInPortal = true;
            interactButton.GetComponent<Image>().enabled = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInPortal= false;
            interactButton.GetComponent<Image>().enabled = false;
        }
    }
}
