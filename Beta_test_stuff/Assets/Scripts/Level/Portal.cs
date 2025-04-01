using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    private bool oneTime = true;
    Sounds soundScript;
    SpawnEnemies spawnEnemiesScript;
    private TextMeshProUGUI enterSignal;
    private bool isInPortal = false;
    Transform player;

    private void Start()
    {
        enterSignal=GameObject.FindGameObjectWithTag("PickUpSignal").GetComponent<TextMeshProUGUI>();
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
        if(Input.GetKeyDown(KeyCode.F) && oneTime && isInPortal && !player.GetComponent<Inventory>().usingWeapon)
        {
            enterSignal.text = "";
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
            enterSignal.text = "Press F to enter portal";
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInPortal= false;
            enterSignal.text = "";
        }
    }
}
