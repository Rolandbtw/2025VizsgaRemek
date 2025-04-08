using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [Header("UI objects")]
    [SerializeField] TextMeshProUGUI dialogueText;
    private GameObject interactButton;
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
        interactButton = GameObject.Find("InteractButton");
        au =GetComponent<AudioSource>();
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
        if (canTalk && movementActions.PlayerMap.Interact.triggered)
        {
            ShowDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = true;
            interactButton.GetComponent<Image>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = false;
            interactButton.GetComponent<Image>().enabled = false;
            HideDialogue();
            StopAllCoroutines();
        }
    }

    public void ShowDialogue()
    {
        StopAllCoroutines();
        if (isGivingTips)
        {
            int index = Random.Range(0, tipLines.Length);
            StartCoroutine(TypeText(tipLines[index]));
        }
        else
        {
            if (PlayerPrefs.GetInt("deathcount")==0)
            {
                int index = Random.Range(0, firstTimeLines.Length);
                StartCoroutine(TypeText(firstTimeLines[index]));
            }
            else if (PlayerPrefs.GetInt("deathcount")==1)
            {
                int index = Random.Range(0, firstDeathLines.Length);
                StartCoroutine(TypeText(firstDeathLines[index]));
            }
            else if(PlayerPrefs.GetInt("deathcount") != 1 && PlayerPrefs.GetInt("deathcount")<10)
            {
                int index = Random.Range(0, deathLines.Length);
                StartCoroutine(TypeText(PlayerPrefs.GetInt("deathcount")+" "+ deathLines[index]));
            }
            else
            {
                int index = Random.Range(0, deathLines2.Length);
                StartCoroutine(TypeText(PlayerPrefs.GetInt("deathcount") + " " + deathLines2[index]));
            }
        }
    }

    void HideDialogue()
    {
        au.Stop();
        dialogueText.text = "";
        interactButton.GetComponent<Image>().enabled = false;
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
