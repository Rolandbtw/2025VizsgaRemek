using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteControl : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] movingAnimation;
    [SerializeField] Sprite[] idleAnimation;

    [Header("Animation settings")]
    public float movingAnimSpeed = 0.1f;
    public float idleAnimSpeed = 0.25f;
    [SerializeField] private bool hasIdleAnimation;
    public bool shouldFacePlayer = true;

    [Header("Needs to be accessed")]
    public bool isMoving;

    private float stepTimer;
    private int currentFrame = 0;
    private Transform player;
    private SpriteRenderer enemyRenderer;

    private void Start()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;

        if (isMoving)
        {
            if (stepTimer >= movingAnimSpeed)
            {
                stepTimer = 0f;
                currentFrame = (currentFrame + 1) % movingAnimation.Length;
                enemyRenderer.sprite = movingAnimation[currentFrame];
            }
        }
        else if (hasIdleAnimation)
        {
            if (stepTimer >= idleAnimSpeed)
            {
                stepTimer = 0f;
                currentFrame = (currentFrame + 1) % idleAnimation.Length;
                enemyRenderer.sprite = idleAnimation[currentFrame];
            }
        }

        if (player.position.x < transform.position.x)
        {
            if (shouldFacePlayer)
            {
                enemyRenderer.flipX = true;
            }
            else
            {
                enemyRenderer.flipX = false;
            }
        }
        else
        {
            if (shouldFacePlayer)
            {
                enemyRenderer.flipX = false;
            }
            else
            {
                enemyRenderer.flipX = true;
            }
        }
    }
}
