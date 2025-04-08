using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] Image ItemPlace1;
    private int placeIndex1;
    private int price1;
    [SerializeField] Image ItemPlace2;
    private int placeIndex2;
    private int price2;

    [SerializeField] Sprite[] itemSprites;
    [SerializeField] GameObject[] itemPickUps;
    [SerializeField] float throwForce;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject portal;
    [SerializeField] Transform portalPoint;


    private bool canOpenShop=false;
    private bool shopIsOpen=false;

    Wallet walletScript;
    Sounds soundScript;
    Runes runeScript;
    private bool oneTime = true;
    float timer;

    private void Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        walletScript=GameObject.FindGameObjectWithTag("Player").GetComponent<Wallet>();
        runeScript=GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyBindings.interact) && canOpenShop && !runeScript.inventoryIsOpened && timer<Time.time) 
        {
            OpenShop();
            timer = Time.time + 1;
        }
        if (Input.GetKeyDown(KeyBindings.runes) && shopIsOpen)
        {
            shopIsOpen = false;
            shopUI.SetActive(false);
            if (oneTime)
            {
                oneTime = false;
                Instantiate(portal, portalPoint.position, portal.transform.rotation);
            }
        }
        if (Input.GetKeyDown(KeyBindings.runes))
        {
            dialogueText.text = "";
        }
    }

    public void RefreshShop()
    {
        oneTime = true;

        int index = Random.Range(0, itemSprites.Length);
        ItemPlace1.sprite = itemSprites[index];
        placeIndex1 = index;
        price1 = Random.Range(5, 70);
        ItemPlace1.GetComponentInChildren<TextMeshProUGUI>().text = price1 + " x";

        index = Random.Range(0, itemSprites.Length);
        ItemPlace2.sprite = itemSprites[index];
        placeIndex2 = index;
        price2 = Random.Range(5, 70);
        ItemPlace2.GetComponentInChildren<TextMeshProUGUI>().text = price2 + " x";
    }

    void OpenShop()
    {
        Time.timeScale = 0;

        dialogueText.text = "";
        shopIsOpen = true;
        shopUI.SetActive(true);
    }

    public void CloseShop()
    {
        Time.timeScale = 1;

        shopIsOpen = false;
        shopUI.SetActive(false);

        if (oneTime)
        {
            Instantiate(portal, portalPoint.position, portal.transform.rotation);
            oneTime = false;
        }
    }

    public void BuyFirst()
    {
        if (walletScript.coins >= price1)
        {
            walletScript.coins -= price1;

            soundScript.MakeSound("buySound", 0.5f);

            GameObject itemClone = Instantiate(itemPickUps[placeIndex1], transform.position, transform.rotation);
            itemClone.GetComponent<Rigidbody2D>().AddForce(transform.right * throwForce, ForceMode2D.Impulse);

            price1 = price1*2;
            ItemPlace1.GetComponentInChildren<TextMeshProUGUI>().text = price1 + " x";

            CloseShop();
        }
        else
        {
            soundScript.MakeSound("errorSound", 0.5f);
        }
    }

    public void BuySecond()
    {
        if (walletScript.coins >= price2)
        {
            walletScript.coins -= price2;

            soundScript.MakeSound("buySound", 0.5f);

            GameObject itemClone = Instantiate(itemPickUps[placeIndex2], transform.position, transform.rotation);
            itemClone.GetComponent<Rigidbody2D>().AddForce(transform.right * throwForce, ForceMode2D.Impulse);

            price2 = price2 * 2;
            ItemPlace2.GetComponentInChildren<TextMeshProUGUI>().text = price2 + " x";

            CloseShop();
        }
        else
        {
            soundScript.MakeSound("errorSound", 0.5f);
        }
    }

    public void BuyThird()
    {
        if (walletScript.coins >= 50)
        {
            walletScript.coins -= 50;
            soundScript.MakeSound("playerHealSound", 0.5f);

            PlayerHealth healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            if (healthScript.healhPoints <= 50)
            {
                healthScript.healhPoints += 50;
            }
            else
            {
                healthScript.healhPoints = 100;
            }

            CloseShop();
        }
        else
        {
            soundScript.MakeSound("errorSound", 0.5f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !shopIsOpen)
        {
            canOpenShop = true;
            dialogueText.text = $"Press {KeyBindings.interact} to open shop";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpenShop = false;
            dialogueText.text = "";
        }
    }
}
