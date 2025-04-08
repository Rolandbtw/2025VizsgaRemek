using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Spear : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    public GameObject spearTrail;
    [Header("Floats to customize")]
    public float damage;
    public float windUpTime;
    public float throwForce;
    public float pullBackDistance;
    public float rotationSpeed;
    public float comeBackSpeed;
    public float stoppingDistance;
    public float cooldown;

    private float timer;

    private Rigidbody2D rb;
    private LookAtMouse lookAtMouseScript;
    private BasicSlicing slicingScript;
    private Transform parent;
    private Inventory inventoryScript;

    private Vector3 initialPosition;

    private bool isShotOut = false;
    private bool canComeBack = false;
    private bool isTouchingWall=false;

    private Runes runes;
    private CooldownSignal cooldownSignal;
    Sounds soundScript;

    IEnumerator Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        inventoryScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        parent = transform.parent;
        lookAtMouseScript = GetComponent<LookAtMouse>();
        slicingScript = GetComponent<BasicSlicing>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        cooldownSignal = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        timer = Time.time + cooldown * runes.playerUltCooldownMultiplier;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyBindings.ultAttack) && !isShotOut && timer<Time.time && !runes.inventoryIsOpened && Time.timeScale!=0 && !isTouchingWall && !inventoryScript.usingWeapon)
        {
            soundScript.MakeSound("spearSound", 0.5f);
            inventoryScript.usingWeapon = true;
            initialPosition = transform.position;
            StartCoroutine(ShootOutSpear());
        }
        if(Input.GetKeyDown(KeyBindings.ultAttack) && isTouchingWall && isShotOut && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            soundScript.MakeSound("spearSound", 0.5f);
            canComeBack= true;
        }

        if (canComeBack)
        {
            ComeBack();
        }
    }

    void ComeBack()
    {
        Vector2 direction = (parent.position - transform.position).normalized;

        if (Vector2.Distance(transform.position, parent.position) > stoppingDistance)
        {
            rb.velocity = direction * comeBackSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            transform.SetParent(parent);
            transform.localPosition = new Vector3(0, 0, 0);
            lookAtMouseScript.enabled = true;
            slicingScript.enabled = true;
            isShotOut = false;
            canComeBack = false;
            spearTrail.SetActive(false);
            inventoryScript.usingWeapon = false;
            timer = Time.time + cooldown * runes.playerUltCooldownMultiplier; // runes
            cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        }
    }

    private IEnumerator ShootOutSpear()
    {
        Vector3 backwardTarget = initialPosition - transform.right * pullBackDistance*-1;

        float elapsedTime = 0f;
        while (elapsedTime < windUpTime)
        {
            transform.position = Vector3.Lerp(initialPosition, backwardTarget, elapsedTime / windUpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lookAtMouseScript.enabled = false;
        slicingScript.enabled = false;
        isShotOut = true;
        transform.position = backwardTarget;
        rb.isKinematic = false;
        transform.SetParent(null);
        spearTrail.SetActive(true);
        rb.AddForce(transform.right * throwForce*-1, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
        {
            isTouchingWall = true;
            rb.velocity = Vector3.zero;
        }
        if (collision.CompareTag("Enemy") && isShotOut && !isTouchingWall)
        {
            collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage*runes.playerMeleeMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
        {
            isTouchingWall = false;
        }
    }
}
