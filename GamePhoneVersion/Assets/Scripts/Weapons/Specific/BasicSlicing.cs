using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlicing : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject slashEffect;
    [Header("Floats to customize")]
    [SerializeField] float damage;
    [SerializeField] float basicAttackCooldown;
    [Header("Animations varriables")]
    public float rotateUpDuration = 1.0f;
    [SerializeField] float rotateDownDuration = 1.5f;
    [SerializeField] float rotateBackDuration = 1.0f;
    [SerializeField] float rotateUpAngle = 80.0f;
    [SerializeField] float rotateDownAngle = 120.0f;
    [Header("Public because needs to be accessed")]
    public Quaternion originalRotation;
    public int rotationDirection;
    public float timer;
    public bool isSlicing = false;
    public bool isUsingAimAssist;

    private float cooldown;
    private Inventory inventoryScript;

    private Runes runes;
    private Sounds soundScript;
    private float triggerTimer;

    private PlayerMovementInputActions movementActions;
    float xInput=1;

    private void Awake()
    {
        if (movementActions == null)
        {
            movementActions = new PlayerMovementInputActions();
        }
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        soundScript =GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes=GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        cooldown = rotateUpDuration + rotateDownDuration + rotateBackDuration+basicAttackCooldown;
        inventoryScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public void Update()
    {
        if (movementActions.PlayerMap.Attack.triggered && timer<Time.time && !runes.inventoryIsOpened && Time.timeScale!=0 && !inventoryScript.usingWeapon)
        {
            StartSlice();
        }

        if (movementActions.PlayerMap.Movement.ReadValue<Vector2>().x != 0)
        {
            xInput = movementActions.PlayerMap.Movement.ReadValue<Vector2>().x;
        }

        if(!isUsingAimAssist)
        {
            if (xInput < 0)
            {
                rotationDirection = -1;
            }
            else if (xInput > 0)
            {
                rotationDirection = 1;
            }
        }
    }

    public void StartSlice()
    {
        GetComponent<LookAtMouse>().enabled = false;
        StartCoroutine(RotateObjectCoroutine());
        timer = Time.time + cooldown;
    }

    IEnumerator RotateObjectCoroutine()
    {
        inventoryScript.usingWeapon = true;
        originalRotation = transform.rotation;

        Quaternion targetUpRotation = originalRotation * Quaternion.Euler(0, 0, rotateUpAngle * rotationDirection);
        yield return StartCoroutine(RotateOverTime(originalRotation, targetUpRotation, rotateUpDuration));

        isSlicing = true;
        slashEffect.SetActive(true);
        soundScript.MakeSound("slicingSound", 0.5f);

        Quaternion currentRotation = transform.rotation;
        Quaternion targetDownRotation = currentRotation * Quaternion.Euler(0, 0, -rotateDownAngle * rotationDirection);
        yield return StartCoroutine(RotateOverTime(currentRotation, targetDownRotation, rotateDownDuration));

        isSlicing = false;
        slashEffect.SetActive(false);

        yield return StartCoroutine(RotateOverTime(transform.rotation, originalRotation, rotateBackDuration));
        GetComponent<LookAtMouse>().enabled= true;
        inventoryScript.usingWeapon = false;
    }

    IEnumerator RotateOverTime(Quaternion fromRotation, Quaternion toRotation, float duration)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = toRotation;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isSlicing && triggerTimer<Time.time)
        {
            collision.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage*runes.playerMeleeMultiplier); //runes
            triggerTimer = Time.time + 0.1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bomb") && isSlicing)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Bomb>().KnockBackBomb(transform, 30);
        }
    }
}
