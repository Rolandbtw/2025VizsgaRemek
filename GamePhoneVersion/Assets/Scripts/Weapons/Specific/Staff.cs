using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject fireBall;
    [SerializeField] Transform bulletPlace;
    [SerializeField] GameObject bulletPrefab;
    [Header("Floats to customize")]
    [SerializeField] float ultDamage;
    [SerializeField] float cooldown;
    [SerializeField] float ultCooldown;
    [SerializeField] float shootingForce;
    [SerializeField] float damage;

    private float timer;
    private float ultTimer;
    private Transform fireBallPosition;

    private Runes runes;
    private CooldownSignal cooldownSignal;
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

    IEnumerator Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        fireBallPosition = GameObject.FindGameObjectWithTag("FireBallPosition").transform;

        cooldownSignal = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(ultCooldown * runes.playerUltCooldownMultiplier);
        ultTimer = Time.time + ultCooldown * runes.playerUltCooldownMultiplier;
    }

    private void Update()
    {
        if (movementActions.PlayerMap.UltAttack.triggered && ultTimer<Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            soundScript.MakeSound("meteorSpawnSound", 0.5f);
            GameObject fireBallClone=Instantiate(fireBall, fireBallPosition.position, fireBall.transform.rotation);
            fireBallClone.GetComponent<FireBall>().damage = ultDamage * runes.playerUltMultiplier; // runes
            fireBallClone.GetComponent<FireBall>().joystickVector = GetComponent<LookAtMouse>().dir;
            ultTimer = Time.time + ultCooldown*runes.playerUltCooldownMultiplier; // runes
            cooldownSignal.PlayCooldownAnim(ultCooldown * runes.playerUltCooldownMultiplier);
        }
        if (movementActions.PlayerMap.Attack.triggered && timer < Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            Shoot();
            timer = Time.time + cooldown;
        }
        else if (movementActions.PlayerMap.Attack.ReadValue<float>() > 0 && timer < Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            Shoot();
            timer = Time.time + cooldown;
        }
    }

    public void Shoot()
    {
        soundScript.MakeSound("staffSound", 0.25f);
        GameObject bullet = Instantiate(bulletPrefab, bulletPlace.position, transform.rotation);
        Bullets bulletsScript = bullet.GetComponent<Bullets>();
        if(bulletsScript != null)
        {
            bulletsScript.damage = damage * runes.playerRangedMultiplier; // runes
        }
        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * -1 * shootingForce, ForceMode2D.Impulse);
    }
}
