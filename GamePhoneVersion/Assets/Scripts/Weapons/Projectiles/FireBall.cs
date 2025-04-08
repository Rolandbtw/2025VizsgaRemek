using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject fireBallEffect;
    [SerializeField] GameObject fireWave;
    private Vector3 targetPosition;
    public Vector3 joystickVector;
    [Header("Floats to customize")]
    [SerializeField] float fallSpeed;
    [SerializeField] float locationThreshold = 0.1f;
    [SerializeField] float rotationSpeed;
    public float damage;

    private Vector3 direction;
    Sounds soundScript;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        movementActions = new PlayerMovementInputActions();
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    void Start()
    {
        GameObject closestEnemy = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetComponent<LookAtMouse>().GetClosestVisibleObjectWithTag("Enemy");
        if (PlayerPrefs.GetInt("AimAssist") == 0 || closestEnemy==null)
        {
            targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(joystickVector.x * 5, joystickVector.y * 5, 0);
        }
        else
        {
            targetPosition=closestEnemy.transform.position;
        }

        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        direction = (targetPosition - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * fallSpeed * Time.deltaTime;

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime*-1);

        if (Vector3.Distance(transform.position, targetPosition) < locationThreshold || transform.position.y< targetPosition.y)
        {
            soundScript.MakeSound("meteorSound", 0.5f);
            Instantiate(fireBallEffect, transform.position, fireBallEffect.transform.rotation);
            GameObject wave=Instantiate(fireWave, transform.position, fireBallEffect.transform.rotation);
            ImpactWave impactScript = wave.GetComponent<ImpactWave>();
            if (impactScript != null)
            {
                impactScript.damage = damage;
            }
            Destroy(gameObject);
        }
    }
}
