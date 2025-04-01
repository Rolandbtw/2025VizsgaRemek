using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    [Header("Items it can contain")]
    [SerializeField] GameObject[] weaponList;
    [SerializeField] GameObject[] runeList;
    [SerializeField] GameObject[] rareItems;

    [Header("Animation and other effect")]
    [SerializeField] GameObject spawnEffect;
    [SerializeField] GameObject openEffect;
    [SerializeField] Sprite openChest;

    [Header("Item throw force")]
    [SerializeField] float itemThrowForce;

    [Header("Other variables")]
    [SerializeField] bool onlyWeapon = false;
    [SerializeField] bool onlyRune = false;
    [SerializeField] LayerMask raycastLayer;
    public bool isRare = false; // needs to be accessed

    private bool canOpenChest = false;
    private bool isOpened=false;
    private bool isTouchingOthers = false;

    private TextMeshProUGUI openChestText;
    private SpriteRenderer chestRenderer;

    Sounds soundScript;

    void Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        openChestText=GameObject.FindGameObjectWithTag("PickUpSignal").GetComponent<TextMeshProUGUI>();
        chestRenderer = GetComponent<SpriteRenderer>();

        CheckPositionCollision(0);
    }

    void CheckPositionCollision(int attempts)
    {
        Vector3 playerPos=GameObject.FindGameObjectWithTag("Player").transform.position;

        isTouchingOthers = false;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0);
        foreach (var collider in hitColliders)
        {
            if (collider != null && (collider.tag == "Clear" || collider.tag == "Portal") && collider.gameObject != gameObject)
            {
                isTouchingOthers = true;
            }
        }

        if (isTouchingOthers && attempts<10)
        {
            Vector3 pos = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().RandomPoint(playerPos, 10);
            transform.position = pos;
            CheckPositionCollision(attempts+1);
        }
        else
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.up * -1, 1, raycastLayer);
            if (hit.Length > 0)
            {
                if (hit[0].collider.gameObject.tag == "wall")
                {
                    transform.position += new Vector3(0, 2, 0);
                }
            }

            Instantiate(spawnEffect, transform.position, transform.rotation);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            openChestText.text = "Press "+ KeyBindings.interact.ToString() + " to open the chest";
            canOpenChest = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            openChestText.text = "";
            canOpenChest = false;
        }
    }

    private void Update()
    {
        if (canOpenChest && !isOpened && Input.GetKeyDown(KeyBindings.interact))
        {
            soundScript.MakeSound("chestSound", 0.25f);
            int weaponOrRune = 0;
            if (onlyRune)
            {
                weaponOrRune = 0;
            }
            else if (onlyWeapon)
            {
                weaponOrRune= 2;
            }
            else if (isRare)
            {
                weaponOrRune = 3;
            }
            else
            {
                weaponOrRune = Random.Range(0, 3);
            }
            GameObject item = null;
            if (weaponOrRune == 0 || weaponOrRune==1)
            {
                if (onlyRune)
                {
                    string weapon = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().currentWeapon;
                    if (weapon == "Sword" || weapon == "Spear" || weapon == "Hammer")
                    {
                        item = runeList[1];
                    }
                    else if (weapon == "")
                    {
                        item = runeList[2];
                    }
                    else
                    {
                        item = runeList[0];
                    }
                }
                else
                {
                    int randomRuneIndex = Random.Range(0, runeList.Length);
                    item = runeList[randomRuneIndex];
                }
            }
            else if(weaponOrRune==2)
            {
                int randomWeaponIndex = Random.Range(0, weaponList.Length);
                item = weaponList[randomWeaponIndex];
            }
            else if(weaponOrRune==3)
            {
                int randomRareItemIndex = Random.Range(0, rareItems.Length);
                item = rareItems[randomRareItemIndex];
            }
            GameObject itemClone=Instantiate(item, transform.position, item.transform.rotation);
            itemClone.GetComponent<Rigidbody2D>().AddForce(transform.up * itemThrowForce * -1, ForceMode2D.Impulse);
            Instantiate(openEffect, transform.position, openEffect.transform.rotation);
            openChestText.text = "";
            chestRenderer.sprite = openChest;
            isOpened = true;
        }
    }
}
