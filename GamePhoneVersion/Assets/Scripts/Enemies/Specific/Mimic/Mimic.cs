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

    [Header("Damage effects")]
    [SerializeField] GameObject blood;
    [SerializeField] GameObject damageEffect;

    private SpriteRenderer mimicRenderer;
    private bool canOpenChest = false;
    private bool isOpened = false;
    private bool isDead = false;
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
        rb =GetComponent<Rigidbody2D>();
        interactButton = GameObject.FindGameObjectWithTag("Interact");
        mimicRenderer =GetComponent<SpriteRenderer>();
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
