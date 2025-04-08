using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteControl : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] Sprite[] idleAnimation;
    [SerializeField] Sprite[] movingAnimation;

    [Header("Animation settings")]
    [SerializeField] float idleAnimSpeed = 0.5f;
    [SerializeField] float movingAnimSpeed = 0.1f;
    [SerializeField] float stepCooldown;

    [Header("Bools")]
    public bool isWalking = false; // needs to be accessed

    float stepTimer;
    int currentFrame = 0;
    Runes runes;
    AudioSource au;

    private PlayerMovementInputActions movementActions;
    bool isMovingRight;

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
        au= GetComponent<AudioSource>();
        runes=GetComponent<Runes>();
    }

    private void Update()
    {
        float xInput = movementActions.PlayerMap.Movement.ReadValue<Vector2>().x;

        if (xInput > 0)
        {
            isMovingRight = true;
        }
        else if(xInput<0)
        {
            isMovingRight = false;
        }

        if (!runes.inventoryIsOpened)
        {
            stepTimer += Time.deltaTime;
            if (isWalking)
            {
                if (stepTimer >= movingAnimSpeed)
                {
                    if (currentFrame == 3)
                    {
                        au.Play();
                    }
                    stepTimer = 0f;
                    currentFrame = (currentFrame + 1) % movingAnimation.Length;
                    playerRenderer.sprite = movingAnimation[currentFrame];
                }
            }
            else
            {
                if (stepTimer >= idleAnimSpeed)
                {
                    stepTimer = 0f;
                    currentFrame = (currentFrame + 1) % idleAnimation.Length;
                    playerRenderer.sprite = idleAnimation[currentFrame];
                }
            }

            if (isMovingRight)
            {
                playerRenderer.flipX = false;
            }
            else if (!isMovingRight)
            {
                playerRenderer.flipX = true;
            }
        }
    }
}
