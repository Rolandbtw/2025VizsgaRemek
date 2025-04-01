using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    public GameObject wave;
    public Transform impactPoint;
    [Header("Floats to customize")]
    public float damage;
    public float knockBackForce;
    public float slamCooldown;
    [Header("Animation varriables")]
    public float rotationDuration;
    public float upMotionDuration;
    public float downMotionDuration;
    public float moveBackDuration;
    public float upDistance;
    public float downDistance;
    public float waitBetweenMovesDuration;

    private float cooldown;
    private float timer;

    private BasicSlicing slicingScript;
    private Inventory inventoryScript;
    private Runes runes;
    private CooldownSignal cooldownSignal;
    Sounds soundScript;
    GameObject player;

    IEnumerator Start()
    {
        soundScript=GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        player = GameObject.FindGameObjectWithTag("Player");
        runes =player.GetComponent<Runes>();
        cooldown = rotationDuration + upMotionDuration + downMotionDuration + moveBackDuration + rotationDuration + (waitBetweenMovesDuration * 4)+slamCooldown;
        slicingScript = GetComponent<BasicSlicing>();
        inventoryScript = player.GetComponent<Inventory>();

        cooldownSignal = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        timer = Time.time + cooldown * runes.playerUltCooldownMultiplier;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyBindings.ultAttack) && slicingScript.timer<Time.time && timer<Time.time && !runes.inventoryIsOpened && Time.timeScale!=0) 
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().isKinematic=true;
            player.GetComponent<SpriteControl>().isWalking=false;
            player.GetComponent<PlayerHealth>().canTakeKnockBack = false;
            GetComponent<LookAtMouse>().enabled = false;
            StartCoroutine(SlamDown());
            timer = Time.time + cooldown*runes.playerUltCooldownMultiplier; //runes
            cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
            GetComponent<BasicSlicing>().timer = Time.time + rotationDuration + upMotionDuration + downMotionDuration + moveBackDuration + rotationDuration + (waitBetweenMovesDuration * 4);
        }
    }

    private IEnumerator SlamDown()
    {
        inventoryScript.usingWeapon = true;

        Quaternion originalRotation = transform.rotation;
        Vector3 originalOffset = transform.position - player.transform.position;

        Quaternion targetRotation = Quaternion.Euler(0, 0, -90);
        yield return StartCoroutine(RotateOverTime(originalRotation, targetRotation, rotationDuration));

        yield return new WaitForSeconds(waitBetweenMovesDuration);

        Vector3 targetOffset = originalOffset + Vector3.up * upDistance;
        yield return StartCoroutine(MoveOverTime(originalOffset, targetOffset, upMotionDuration));

        yield return new WaitForSeconds(waitBetweenMovesDuration);

        Vector3 downOffset = originalOffset + Vector3.down * downDistance;
        yield return StartCoroutine(MoveOverTime(targetOffset, downOffset, downMotionDuration));
        SpawnImpactWave();

        yield return new WaitForSeconds(waitBetweenMovesDuration);

        yield return StartCoroutine(MoveOverTime(downOffset, originalOffset, moveBackDuration));

        yield return new WaitForSeconds(waitBetweenMovesDuration);

        yield return StartCoroutine(RotateOverTime(targetRotation, originalRotation, rotationDuration));

        GetComponent<LookAtMouse>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerHealth>().canTakeKnockBack = true;
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        inventoryScript.usingWeapon = false;
    }

    private IEnumerator RotateOverTime(Quaternion fromRotation, Quaternion toRotation, float duration)
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

    private IEnumerator MoveOverTime(Vector3 fromOffset, Vector3 toOffset, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            Vector3 playerPosition = player.transform.position;
            transform.position = Vector3.Lerp(
                playerPosition + fromOffset,
                playerPosition + toOffset,
                timeElapsed / duration
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = player.transform.position + toOffset;
    }

    void SpawnImpactWave()
    {
        soundScript.MakeSound("hammerSound", 0.5f);
        GameObject impactWave=Instantiate(wave, impactPoint.position, wave.transform.rotation);
        impactWave.GetComponent<ImpactWave>().damage = damage;
        impactWave.GetComponent<ImpactWave>().knockBackForce = knockBackForce;
    }
}
