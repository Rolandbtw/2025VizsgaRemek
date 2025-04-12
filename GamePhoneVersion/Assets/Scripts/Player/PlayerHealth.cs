using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class PlayerHealth : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] Image healthImage;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject blood;
    [SerializeField] GameObject weaponPlace;

    [Header("Floats to customize")]
    public float healhPoints; // needs to be accessed
    [SerializeField] float damageCooldown;
    [SerializeField] float knockBackStunDuration;

    [Header("Can take knockback")]
    public bool canTakeKnockBack = true; // needs to be accessed

    [Header("Death objects")]
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] GameObject deathMessage;

    float timer;
    bool dieOnlyOnce = true;
    bool isDead = false;

    Runes runes;
    Rigidbody2D rb;
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

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes =GetComponent<Runes>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (healhPoints <= 0 && dieOnlyOnce)
        {
            Death();
            dieOnlyOnce = false;
            isDead = true;
        }
        if (healhPoints <= 0 && movementActions.PlayerMap.Retry.triggered)
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        LerpHealt();
        healthText.text = "HP: " + MathF.Round(healhPoints, 2);
    }

    void LerpHealt()
    {
        float targetAlpha = Mathf.Lerp(40f / 255f, 0, healhPoints / 100);
        Color newColor = healthImage.color;
        newColor.a = targetAlpha;
        healthImage.color = newColor;
    }

    void Death()
    {
        gameObject.layer = LayerMask.NameToLayer("Dash");

        PlayerPrefs.SetInt("deathcount", PlayerPrefs.GetInt("deathcount") + 1);
        if (PlayerPrefs.GetString("userName") != "Guest")
        {
            StartCoroutine(Save(PlayerPrefs.GetInt("id"), PlayerPrefs.GetInt("deathcount")));
        }

        Vector3 particlePosition = new Vector3(transform.position.x + 0.75f, transform.position.y, transform.position.z);
        Instantiate(damageEffect, transform.position, transform.rotation);
        Instantiate(blood, transform.position, transform.rotation);
        Instantiate(deathParticle, particlePosition, transform.rotation);

        weaponPlace.SetActive(false);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;

        StartCoroutine(DeathWait());
    }

    IEnumerator DeathWait()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1;
        deathMessage.SetActive(true);
        SpawnEnemies sc = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        StartCoroutine(sc.DeathCircleWipe());
    }

    public void DamagePlayer(float damage, bool isMelee)
    {
        if (timer < Time.time && !isDead)
        {
            StartCoroutine(GetComponent<Runes>().TakesDamage());
            soundScript.MakeSound("playerHurtSound", 0.5f);
            StartCoroutine(DamageFlash());
            if (isMelee)
            {
                if (healhPoints - damage * runes.playerMeleeResistance < 0) // runes
                {
                    healhPoints = 0;
                }
                else
                {
                    healhPoints -= damage * runes.playerMeleeResistance; // runes
                }
            }
            else
            {
                if (healhPoints - damage * runes.playerRangedResistance < 0) // runes
                {
                    healhPoints = 0;
                }
                else
                {
                    healhPoints -= damage * runes.playerRangedResistance; // runes
                }
            }
            Instantiate(damageEffect, transform.position, damageEffect.transform.rotation);
            Instantiate(blood, transform.position, transform.rotation);
            timer = Time.time + damageCooldown;
        }
    }

    public void KnockBackPlayer(float force, Transform enemy)
    {
        if (canTakeKnockBack)
        {
            rb.drag = 2.5f;
            rb.velocity = Vector2.zero;
            GetComponent<PlayerMovement>().enabled = false;
            Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
            rb.AddForce(knockbackDirection * force, ForceMode2D.Impulse);
            StartCoroutine(KnockBackStun());
        }
    }

    IEnumerator KnockBackStun()
    {
        yield return new WaitForSeconds(knockBackStunDuration);
        rb.velocity = Vector2.zero;
        rb.drag = 0;
        if (!isDead)
        {
            GetComponent<PlayerMovement>().enabled = true;
        }
    }

    IEnumerator DamageFlash()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator Save(int id, int death)
    {
        string json = "{\"deaths\": " + death +
                ",\"boss1lvl\": " + PlayerPrefs.GetInt("boss1lvl") +
                ",\"boss2lvl\": " + PlayerPrefs.GetInt("boss2lvl") +
                ",\"boss3lvl\": " + PlayerPrefs.GetInt("boss3lvl") +
                ",\"kills\": " + PlayerPrefs.GetInt("kills");

        if (PlayerPrefs.GetInt("waves") > PlayerPrefs.GetInt("bestWaves"))
        {
            json += ",\"waves\": " + PlayerPrefs.GetInt("waves");
        }
        json += "}";

        Debug.Log(json);

        using (UnityWebRequest request = UnityWebRequest.Put($"http://localhost:8000/api/user/update/{id}", json))
        {
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
