using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("GameObjects and Transforms and Sprites")]
    [SerializeField] GameObject defaultArrow;
    [SerializeField] GameObject windUpArrow;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject defaultArrowUlt;
    [SerializeField] GameObject windUpArrowUlt;
    [SerializeField] Sprite defaultBow;
    [SerializeField] Sprite windUpBow;
    [Header("Floats to customize")]
    [SerializeField] float damage;
    [SerializeField] float shootingForce;
    [SerializeField] float windUpDuration;
    [SerializeField] float cooldown;

    private SpriteRenderer bowRenderer;
    [SerializeField] bool isWindedUp = false;
    [SerializeField] bool isWindedUpUlt=false;
    private float timer;

    private Inventory inventoryScript;
    private Runes runes;
    private CooldownSignal cooldownSignal;
    Sounds soundScript;

    private Coroutine windUp=null;
    private Coroutine windUpUlt=null;

    IEnumerator Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        bowRenderer = GetComponentInChildren<SpriteRenderer>();
        inventoryScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        cooldownSignal = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        timer = Time.time + cooldown * runes.playerUltCooldownMultiplier;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyBindings.attack) && !runes.inventoryIsOpened && Time.timeScale != 0 && !inventoryScript.usingWeapon)
        {
            windUp=StartCoroutine(WindUpBow());
        }
        if (Input.GetKeyUp(KeyBindings.attack) && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            if(isWindedUp)
            {
                soundScript.MakeSound("bowSound", 0.5f);
                windUp = null;
                ShootArrow();
            }
            else
            {
                if (windUp != null)
                {
                    StopCoroutine(windUp);
                    windUp = null;
                }
                if (windUpUlt==null)
                {
                    inventoryScript.usingWeapon = false;
                }
                isWindedUp = false;
                defaultArrow.SetActive(false);
                windUpArrow.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyBindings.ultAttack) && timer<Time.time && !runes.inventoryIsOpened && !inventoryScript.usingWeapon && Time.timeScale != 0)
        {
            windUpUlt=StartCoroutine(WindUpBowUlt());
        }
        if (Input.GetKeyUp(KeyBindings.ultAttack) && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            if (isWindedUpUlt)
            {
                soundScript.MakeSound("bowSound", 0.5f);
                windUpUlt = null;
                ShootArrowUlt();
                timer = Time.time + cooldown * runes.playerUltCooldownMultiplier; // runes
                cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
            }
            else
            {
                if (windUpUlt != null)
                {
                    StopCoroutine(windUpUlt);
                    windUpUlt = null;
                }
                if (windUp == null)
                {
                    inventoryScript.usingWeapon = false;
                }
                isWindedUpUlt = false;
                defaultArrowUlt.SetActive(false);
                windUpArrowUlt.SetActive(false);
            }
        }
    }

    IEnumerator WindUpBow()
    {
        inventoryScript.usingWeapon = true;
        yield return new WaitForSeconds(windUpDuration/2);
        defaultArrow.SetActive(true);
        yield return new WaitForSeconds(windUpDuration/2);
        bowRenderer.sprite = windUpBow;
        defaultArrow.SetActive(false);
        windUpArrow.SetActive(true);
        isWindedUp = true;
    }

    void ShootArrow()
    {
        isWindedUp = false;
        bowRenderer.sprite = defaultBow;
        GameObject arrow=Instantiate(arrowPrefab, windUpArrow.transform.position, transform.rotation);
        arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.right * -1 * shootingForce, ForceMode2D.Impulse);
        arrow.GetComponent<Bullets>().damage = damage*runes.playerRangedMultiplier; // runes
        windUpArrow.SetActive(false);
        inventoryScript.usingWeapon = false;
    }

    IEnumerator WindUpBowUlt()
    {
        inventoryScript.usingWeapon = true;
        yield return new WaitForSeconds(windUpDuration / 2);
        defaultArrowUlt.SetActive(true);
        yield return new WaitForSeconds(windUpDuration / 2);
        bowRenderer.sprite = windUpBow;
        defaultArrowUlt.SetActive(false);
        windUpArrowUlt.SetActive(true);
        isWindedUpUlt = true;
    }

    void ShootArrowUlt()
    {
        isWindedUpUlt = false;
        bowRenderer.sprite = defaultBow;
        int rotationOffset = -25;
        for (int i = 0; i < 3; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, windUpArrow.transform.position, transform.rotation);
            Vector3 currentRotation = arrow.transform.rotation.eulerAngles;
            currentRotation.z += rotationOffset;
            arrow.transform.rotation = Quaternion.Euler(currentRotation);
            arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.right * -1 * shootingForce, ForceMode2D.Impulse);
            arrow.GetComponent<Bullets>().damage = damage * runes.playerRangedMultiplier; // runes
            rotationOffset += 25;
        }
        windUpArrowUlt.SetActive(false);
        inventoryScript.usingWeapon = false;
    }
}
