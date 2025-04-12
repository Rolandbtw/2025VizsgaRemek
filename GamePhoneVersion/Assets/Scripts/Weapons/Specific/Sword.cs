using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class Sword : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] Transform slashWavePositon;
    [SerializeField] GameObject slashWave;
    [Header("Floats to customize")]
    [SerializeField] float cooldown;
    [SerializeField] float damage;
    [SerializeField] float knockbakcForce;
    [SerializeField] float waveMoveForce;

    private Vector3 originalPosition;
    private float timer;

    private BasicSlicing sc;
    private Runes runes;
    private CooldownSignal cooldownSignal;
    Sounds soundScript;

    private PlayerMovementInputActions movementActions;

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

    IEnumerator Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
        sc = GetComponent<BasicSlicing>();

        cooldownSignal =GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        timer = Time.time + cooldown * runes.playerUltCooldownMultiplier;
    }

    public void Update()
    {
        if(movementActions.PlayerMap.UltAttack.triggered && sc.timer < Time.time && timer < Time.time && !runes.inventoryIsOpened && Time.timeScale != 0)
        {
            originalPosition=slashWavePositon.position;
            sc.StartSlice();
            StartCoroutine(WaitForSlashSpawn());
            timer = Time.time + cooldown*runes.playerUltCooldownMultiplier; // runes
            cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        }
    }

    IEnumerator WaitForSlashSpawn()
    {
        yield return new WaitForSeconds(sc.rotateUpDuration);
        soundScript.MakeSound("swordSound", 0.5f);
        GameObject wave = Instantiate(slashWave, originalPosition, sc.originalRotation);
        wave.GetComponent<Rigidbody2D>().AddForce(wave.transform.right * -1 * waveMoveForce, ForceMode2D.Impulse);
        wave.GetComponent<ImpactWave>().damage = damage * runes.playerUltMultiplier;
    }
}
