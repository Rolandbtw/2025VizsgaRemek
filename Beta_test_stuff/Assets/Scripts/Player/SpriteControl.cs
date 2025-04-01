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

    private void Start()
    {
        au= GetComponent<AudioSource>();
        runes=GetComponent<Runes>();
    }

    private void Update()
    {
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

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x > transform.position.x)
            {
                playerRenderer.flipX = false;
            }
            else if (mousePos.x < transform.position.x)
            {
                playerRenderer.flipX = true;
            }
        }
    }
}
