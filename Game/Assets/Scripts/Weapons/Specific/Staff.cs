using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    public GameObject fireBall;
    public Transform bulletPlace;
    public GameObject bulletPrefab;
    [Header("Floats to customize")]
    public float ultDamage;
    public float cooldown;
    public float ultCooldown;
    public float shootingForce;
    public float damage;

    private float timer;
    private float ultTimer;
    private Transform fireBallPosition;

    private Runes runes;
    private CooldownSignal cooldownSignal;
    Sounds soundScript;

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
        if (Input.GetKeyDown(KeyBindings.ultAttack) && ultTimer<Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            soundScript.MakeSound("meteorSpawnSound", 0.5f);
            GameObject fireBallClone=Instantiate(fireBall, fireBallPosition.position, fireBall.transform.rotation);
            fireBallClone.GetComponent<FireBall>().damage = ultDamage;
            ultTimer = Time.time + ultCooldown*runes.playerUltCooldownMultiplier; // runes
            cooldownSignal.PlayCooldownAnim(ultCooldown * runes.playerUltCooldownMultiplier);
        }
        if (Input.GetKey(KeyBindings.attack) && timer < Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
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
