using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class PlayerMovement : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] TrailRenderer dashEffect;

    [Header("Movement varriables")]
    public float moveSpeed = 5f; // needs to be accessed
    [SerializeField] float smoothing = 0.1f;

    [Header("Dodge varriables")]
    [SerializeField] float dodgeSpeed = 15f;
    [SerializeField] float dodgeDuration = 0.2f;
    [SerializeField] float dodgeCooldown = 1f;
    [SerializeField] bool isDodging = false;

    float dodgeEndTime;
    float nextDodgeTime;
    LayerMask playerLayer;
    LayerMask dashLayer;
    Vector2 movementInput;
    Vector2 currentVelocity = Vector2.zero;
    Rigidbody2D rb;
    Sounds soundScript;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        movementActions= new PlayerMovementInputActions();
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
        movementActions.PlayerMap.Dash.performed += ctx => Dodge();
    }

    void Start()
    {
        soundScript =GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        dashLayer = LayerMask.NameToLayer("Dash");
        playerLayer = LayerMask.NameToLayer("Player");
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        movementInput = movementActions.PlayerMap.Movement.ReadValue<Vector2>();
        movementInput.Normalize();

        if (movementInput==new Vector2(0, 0))
        {
            GetComponent<SpriteControl>().isWalking = false;
        }
        else 
        {
            GetComponent<SpriteControl>().isWalking = true;
        }

        if (isDodging && Time.time >= dodgeEndTime)
        {
            EndDodge();
        }
    }

    void Dodge()
    {
        if (Time.time>=nextDodgeTime && movementInput!=new Vector2(0, 0)) 
        {
            StartDodge();
            soundScript.MakeSound("playerDashSound", 0.5f);
        }
    }

    void FixedUpdate()
    {
        if (!isDodging)
        {
            Vector2 targetVelocity = movementInput * moveSpeed*GetComponent<Runes>().playerSpeedMultiplier;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothing);
        }
    }

    void StartDodge()
    {
        gameObject.layer = dashLayer;
        dashEffect.emitting = true;
        isDodging = true;
        dodgeEndTime = Time.time + dodgeDuration;
        nextDodgeTime = Time.time + dodgeCooldown;
        rb.velocity = movementInput * dodgeSpeed;
    }

    void EndDodge()
    {
        gameObject.layer = playerLayer;
        isDodging = false;
        rb.velocity = Vector2.zero;
        dashEffect.emitting = false;
    }
}
