using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mimic : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] sprites;

    [Header("Animation settings")]
    [SerializeField] private float movingAnimSpeed = 0.1f;
    [SerializeField] private float stepCooldown;
    [SerializeField] private float stepTimer;
    [SerializeField] private int currentFrame = 0;

    [Header("Knockback settings")]
    [SerializeField] private float knockBackStrength;

    [Header("Effects")]
    [SerializeField] GameObject blood;
    [SerializeField] GameObject damageEffect;

    [Header("Other")]
    [SerializeField] LayerMask raycastLayer;

    private SpriteRenderer mimicRenderer;
    private bool canOpenChest = false;
    private bool isOpened = false;
    private bool isDead = false;
    private bool isTouchingOthers = false;
    private Rigidbody2D rb;
    Sounds soundScript;
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
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        rb = GetComponent<Rigidbody2D>();
        interactButton = GameObject.FindGameObjectWithTag("Interact");
        mimicRenderer =GetComponent<SpriteRenderer>();

        CheckPositionCollision(0);
    }

    void CheckPositionCollision(int attempts)
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        isTouchingOthers = false;

        Transform portal = GameObject.FindGameObjectWithTag("Portal").transform;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0);
        foreach (var collider in hitColliders)
        {
            if (collider != null && collider.tag == "Clear" && collider.gameObject != gameObject)
            {
                isTouchingOthers = true;
            }
        }

        if (Vector2.Distance(transform.position, portal.position) < 5)
        {
            isTouchingOthers = true;
        }

        if (isTouchingOthers && attempts < 10)
        {
            Vector3 pos = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().RandomPoint(playerPos, 10);
            transform.position = pos;
            CheckPositionCollision(attempts + 1);
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

            Instantiate(damageEffect, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            interactButton.GetComponent<Image>().enabled = true;
            canOpenChest = true;
        }
        if(collision.CompareTag("Weapon") && isOpened && !isDead && collision.gameObject.GetComponent<BasicSlicing>().isSlicing || collision.CompareTag("Bullet") && isOpened && !isDead)
        {
            Instantiate(blood, transform.position, transform.rotation);
            Instantiate(damageEffect, transform.position, transform.rotation);
            GetComponent<BoxCollider2D>().enabled = false;
            isDead = false;
            rb.isKinematic = false;
            rb.gravityScale = 3.0f;
            rb.freezeRotation = false;
            rb.AddTorque(50);
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
            rb.AddForce(randomDirection * 15, ForceMode2D.Impulse);

            soundScript.MakeSound("enemyDeathSound", 0.5f);

            Destroy(gameObject, 10);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            interactButton.GetComponent<Image>().enabled = false;
            canOpenChest = false;
        }
    }

    private void Update()
    {
        if(canOpenChest && !isOpened && movementActions.PlayerMap.Interact.triggered)
        {
            interactButton.GetComponent<Image>().enabled = false;
            PlayerHealth sc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            sc.DamagePlayer(Mathf.RoundToInt(sc.healhPoints/2), true);
            sc.KnockBackPlayer(knockBackStrength, transform);
            isOpened = true;
        }

        if (isOpened)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= movingAnimSpeed)
            {
                stepTimer = 0f;
                currentFrame = (currentFrame + 1) % sprites.Length;
                mimicRenderer.sprite = sprites[currentFrame];
            }
        }
    }
}
