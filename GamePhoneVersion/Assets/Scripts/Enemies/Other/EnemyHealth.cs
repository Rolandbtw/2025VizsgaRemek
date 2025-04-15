using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Blood objects")]
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject icyBlood;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private GameObject iceDamageEffect;
    [Header("Damage popup")]
    [SerializeField] private GameObject damagePopup;
    [Header("Health settings")]
    public float healhPoints;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float knockBackStunDuration;
    [Header("Needs to be accessed")]
    public Color baseColor;
    public bool isDead = false;
    [Header("Drops")]
    [SerializeField] GameObject coin;
    [SerializeField] GameObject health;

    private float timer;
    private Rigidbody2D rb;

    public bool isFroozen = false;

    Sounds soundScript;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        rb = GetComponent<Rigidbody2D>();

        if (name.Contains("Big Mud") || name.Contains("Elf") || name.Contains("LittleGuy"))
        {
            int enemyCount = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().maxEnemyCount;
            healhPoints += (healhPoints * (enemyCount / 100)) * 5;
            GetComponentInChildren<Canvas>().transform.GetComponentInChildren<Slider>().maxValue = healhPoints;
        }
    }

    public void Update()
    {
        if (healhPoints <= 0 && !isDead && !name.Contains("Goblin"))
        {
            isDead = true;
            GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive--;
            DropSomething();

            if (name.Contains("Big Mud") || name.Contains("Elf") || name.Contains("LittleGuy"))
            {
                if (name.Contains("Elf")) { PlayerPrefs.SetInt("boss1lvl", 1); }
                if (name.Contains("LittleGuy")) { PlayerPrefs.SetInt("boss2lvl", 1); }
                if (name.Contains("Big Mud")) { PlayerPrefs.SetInt("boss3lvl", 1); }

                Destroy(transform.GetChild(0).gameObject);
            }

            if (GetComponent<NavMeshAgent>() != null)
            {
                GetComponent<NavMeshAgent>().enabled = false;
            }

            if (isFroozen)
            {
                Instantiate(iceDamageEffect, transform.position, transform.rotation);
            }

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script.GetType().Name != "EnemyHealth" && script.GetType().Name != "Light2D")
                {
                    script.enabled = false;
                }
            }
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                col.enabled = false;
            }

            rb.isKinematic = false;
            rb.gravityScale = 3.0f;
            rb.freezeRotation = false;
            rb.AddTorque(50);
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
            rb.AddForce(randomDirection * 15, ForceMode2D.Impulse);

            soundScript.MakeSound("enemyDeathSound", 0.5f);

            int kills = PlayerPrefs.GetInt("kills");
            kills++;
            PlayerPrefs.SetInt("kills", kills);

            Destroy(gameObject, 10);


        }
        else if (!isDead && healhPoints <= 0)
        {
            int kills = PlayerPrefs.GetInt("kills");
            kills++;
            PlayerPrefs.SetInt("kills", kills);
            isDead = true;
            GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().currentEnemiesAlive--;
        }
    }

    public void DamageEnemy(float damage)
    {

        if (timer < Time.time)
        {
            Vector3 effectPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + 0.75f, transform.position.z);
            GameObject popup = Instantiate(damagePopup, effectPos, transform.rotation);
            popup.GetComponent<DamagePopup>().SetDamageText(damage);
            soundScript.MakeSound("enemyDamageSound", 0.1f);

            if (name.Contains("Goblin"))
            {
                StartCoroutine(GetComponent<SuicideGoblin>().Explosion());
            }

            StartCoroutine(DamageFlash());
            healhPoints -= damage;

            Instantiate(damageEffect, transform.position, transform.rotation);
            if (isFroozen)
            {
                Instantiate(icyBlood, transform.position, transform.rotation);
            }
            else
            {
                Instantiate(blood, transform.position, transform.rotation);
            }
            timer = Time.time + damageCooldown;
        }
        RangedPathfinding sc = GetComponent<RangedPathfinding>();
        if (sc != null && !isDead)
        {
            sc.FleeForYourLife();
        }
    }

    public void FireDamageEnemy(float damage)
    {
        StartCoroutine(FireDamage(damage));
    }

    public void KnockBackEnemy(float force, Transform point)
    {
        rb.isKinematic = false;

        List<MonoBehaviour> scripts = GetComponents<MonoBehaviour>().ToList();
        foreach (MonoBehaviour script in scripts)
        {
            if (GetComponent<MonoBehaviour>().ToString().Contains("Pathfinding"))
            {
                script.enabled = false;
            }
        }

        Vector2 knockbackDirection = (transform.position - point.position).normalized;
        rb.AddForce(knockbackDirection * force, ForceMode2D.Impulse);
        StartCoroutine(KnockBackStun());
    }

    IEnumerator KnockBackStun()
    {
        yield return new WaitForSeconds(knockBackStunDuration);
        if (!isDead)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;

            List<MonoBehaviour> scripts = GetComponents<MonoBehaviour>().ToList();
            foreach (MonoBehaviour script in scripts)
            {
                if (GetComponent<MonoBehaviour>().ToString().Contains("Pathfinding"))
                {
                    script.enabled = true;
                }
            }
        }
    }

    IEnumerator DamageFlash()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = baseColor;
    }

    IEnumerator FireDamage(float damage)
    {
        for (int i = 0; i < 5; i++)
        {
            if (isDead)
            {
                StopAllCoroutines();
            }
            DamageEnemy(damage);
            yield return new WaitForSeconds(2.5f);
        }
    }

    void DropSomething()
    {
        int randomDrop = Random.Range(0, 3);
        if (randomDrop == 0)
        {
            Instantiate(health, transform.position, transform.rotation);
        }
        else
        {
            Instantiate(coin, transform.position, transform.rotation);
        }
    }
}
