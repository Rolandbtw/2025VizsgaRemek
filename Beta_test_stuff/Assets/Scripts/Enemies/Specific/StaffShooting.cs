using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffShooting : MonoBehaviour
{
    [Header("Shooting settings")]
    [SerializeField] private float damage;
    [SerializeField] private float shootingForce;
    [SerializeField] private float shootingCooldown;

    [Header("Transforms")]
    [SerializeField] private Transform weapon;
    [SerializeField] private Transform shootingPoint;

    [Header("Projectile object")]
    [SerializeField] private GameObject bullet;

    [Header("Needs to be accessed")]
    public bool canShoot;

    private Transform target;
    private float timer;

    Sounds soundScript;

    private void Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        timer = Time.time + 1f;
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y * -1, dir.x * -1) * Mathf.Rad2Deg;
        weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (timer < Time.time && canShoot)
        {
            ShootingFunction();
            timer = Time.time + shootingCooldown;
        }
    }

    void ShootingFunction()
    {
        soundScript.MakeSound("wizardShootSound", 0.1f);
        GameObject bulletPrefab=Instantiate(bullet, shootingPoint.position, weapon.rotation);
        bulletPrefab.GetComponent<Bullets>().damage = damage;
        bulletPrefab.GetComponent<Bullets>().isIgnoringEnemy = true;
        bulletPrefab.GetComponent<Rigidbody2D>().AddForce(weapon.right * -1 * shootingForce, ForceMode2D.Impulse);
    }
}
