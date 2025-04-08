using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Header("UI objects")]
    [SerializeField] TextMeshProUGUI talkSignal;
    [SerializeField] GameObject dialogueUI;
    [SerializeField] TextMeshProUGUI dialogueText;
    [Header("Dialogue settings")]
    [SerializeField] string[] tipLines;
    [SerializeField] string[] firstTimeLines;
    [SerializeField] string[] firstDeathLines;
    [SerializeField] string[] deathLines;
    [SerializeField] string[] deathLines2;
    [SerializeField] float waitTime;
    [SerializeField] bool isGivingTips;

    private bool canTalk = false;
    private AudioSource au;

    private void Start()
    {
        au=GetComponent<AudioSource>();
        if (!isGivingTips)
        {
            if (!PlayerPrefs.HasKey("Deathcount"))
            {
                PlayerPrefs.SetInt("Deathcount", 0);
            }
        }
    }

    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyBindings.interact))
        {
            ShowDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = true;
            talkSignal.text = $"Press {KeyBindings.interact} to chat with "+gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = false;
            talkSignal.text = "";
            HideDialogue();
            StopAllCoroutines();
        }
    }

    public void ShowDialogue()
    {
        StopAllCoroutines();
        if (isGivingTips)
        {
            talkSignal.text = "";
            dialogueUI.SetActive(true);
            int index = Random.Range(0, tipLines.Length);
            StartCoroutine(TypeText(tipLines[index]));
        }
        else
        {
            if (PlayerPrefs.GetInt("deathcount")==0)
            {
                talkSignal.text = "";
                dialogueUI.SetActive(true);
                int index = Random.Range(0, firstTimeLines.Length);
                StartCoroutine(TypeText(firstTimeLines[index]));
            }
            else if (PlayerPrefs.GetInt("deathcount")==1)
            {
                talkSignal.text = "";
                dialogueUI.SetActive(true);
                int index = Random.Range(0, firstDeathLines.Length);
                StartCoroutine(TypeText(firstDeathLines[index]));
            }
            else if(PlayerPrefs.GetInt("deathcount") != 1 && PlayerPrefs.GetInt("deathcount")<10)
            {
                talkSignal.text = "";
                dialogueUI.SetActive(true);
                int index = Random.Range(0, deathLines.Length);
                StartCoroutine(TypeText(PlayerPrefs.GetInt("deathcount")+" "+ deathLines[index]));
            }
            else
            {
                talkSignal.text = "";
                dialogueUI.SetActive(true);
                int index = Random.Range(0, deathLines2.Length);
                StartCoroutine(TypeText(PlayerPrefs.GetInt("deathcount") + " " + deathLines2[index]));
            }
        }
    }

    void HideDialogue()
    {
        au.Stop();
        dialogueUI.SetActive(false);
        dialogueText.text = "";
    }

    IEnumerator TypeText(string message)
    {
        au.Play();
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(waitTime);
        }
        au.Stop();
    }
}
