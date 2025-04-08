using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchAnim : MonoBehaviour
{
    private SpriteRenderer torchRenderer;
    [SerializeField] Sprite[] sprites;

    private float stepTimer;
    public float movingAnimSpeed = 0.1f;
    private int currentFrame = 0;

    private void Start()
    {
        torchRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= movingAnimSpeed)
        {
            stepTimer = 0f;
            currentFrame = (currentFrame + 1) % sprites.Length;
            torchRenderer.sprite = sprites[currentFrame];
        }
    }
}
