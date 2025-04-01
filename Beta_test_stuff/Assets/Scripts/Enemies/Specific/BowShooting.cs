using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShooting : MonoBehaviour
{
    [Header("Bow sprites")]
    [SerializeField] private Sprite bow;
    [SerializeField] private Sprite windUpBow;
    [SerializeField] private SpriteRenderer bowRenderer;

    [Header("Arrow sprites")]
    [SerializeField] private GameObject arrow1;
    [SerializeField] private GameObject arrow2;
    [SerializeField] private GameObject ultArrow1;
    [SerializeField] private GameObject ultArrow2;

    [Header("Projectile object")]
    [SerializeField] private GameObject arrowPrefab;

    [Header("Bow transform (turns towards player)")]
    [SerializeField] private Transform weapon;

    [Header("Shooting settings")]
    [SerializeField] private float shootingForce;
    [SerializeField] private float damage;
    [SerializeField] private float shootingCooldown;

    [Header("Needs to be accessed")]
    public bool canShoot;

    private Transform target;
    private float timer;
    private bool isSpecial=false;
    [SerializeField] bool canBeSpecial=false;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (canBeSpecial)
        {
            int shouldBeSpecial = Random.Range(0, 5);
            if (shouldBeSpecial == 1)
            {
                isSpecial = true;
                arrow1.SetActive(false);
                ultArrow1.SetActive(true);
            }
        }

        timer = Time.time + Random.Range(shootingCooldown / 3, shootingCooldown);
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y * -1, dir.x * -1) * Mathf.Rad2Deg;
        weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(timer<Time.time && canShoot) 
        {
            StartCoroutine(ShootingStart());
            timer = Time.time + Random.Range(shootingCooldown/3, shootingCooldown);
        }
    }

    IEnumerator ShootingStart()
    {
        yield return new WaitForSeconds(0.25f);
        bowRenderer.sprite = windUpBow;
        if (isSpecial)
        {
            ultArrow1.SetActive(false);
            ultArrow2.SetActive(true);
        }
        else
        {
            arrow1.SetActive(false);
            arrow2.SetActive(true);
        }
        yield return new WaitForSeconds(0.25f);
        if (isSpecial)
        {
            ShootUltArrow();
            ultArrow1.SetActive(true);
            ultArrow2.SetActive(false);
        }
        else
        {
            ShootArrow();
            arrow2.SetActive(false);
            arrow1.SetActive(true);
        }
        bowRenderer.sprite = bow;
    }

    void ShootArrow()
    {
        soundScript.MakeSound("bowSound", 0.5f);
        GameObject arrow = Instantiate(arrowPrefab, arrow1.transform.position, weapon.rotation);
        arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.right * -1 * shootingForce, ForceMode2D.Impulse);
        if (canBeSpecial)
        {
            arrow.GetComponent<Bullets>().damage = damage;
            arrow.GetComponent<Bullets>().isIgnoringEnemy = true;
        }
    }

    void ShootUltArrow()
    {
        soundScript.MakeSound("bowSound", 0.5f);
        int rotationOffset = -25;
        for (int i = 0; i < 3; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, ultArrow2.transform.position, weapon.rotation);
            Vector3 currentRotation = arrow.transform.rotation.eulerAngles;
            currentRotation.z += rotationOffset;
            arrow.transform.rotation = Quaternion.Euler(currentRotation);
            arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.right * -1 * shootingForce, ForceMode2D.Impulse);
            arrow.GetComponent<Bullets>().damage = damage;
            arrow.GetComponent<Bullets>().isIgnoringEnemy = true;
            rotationOffset += 25;
        }
    }
}
