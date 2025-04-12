using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    [SerializeField] GameObject suriken;
    [SerializeField] float cooldown;
    private float timer;
    private Runes runes;
    private Transform player;
    private CooldownSignal cooldownSignal;
    Runes runesScript;

    IEnumerator Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        runesScript=player.GetComponent<Runes>();
        runes=player.GetComponent<Runes>();

        cooldownSignal = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<CooldownSignal>();
        yield return null;
        cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        timer = Time.time + cooldown * runes.playerUltCooldownMultiplier;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyBindings.ultAttack) && timer<Time.time && Time.timeScale != 0 && !runesScript.inventoryIsOpened) 
        {
            Vector3 surikenPosition = new Vector3(player.position.x, transform.position.y - 0.05f, transform.position.z);
            Instantiate(suriken, surikenPosition, suriken.transform.rotation);
            timer = Time.time+cooldown*runes.playerUltCooldownMultiplier;
            cooldownSignal.PlayCooldownAnim(cooldown * runes.playerUltCooldownMultiplier);
        }
    }
}
