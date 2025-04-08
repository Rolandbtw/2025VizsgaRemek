using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Runes : MonoBehaviour
{
    public float playerMeleeMultiplier;
    public float playerRangedMultiplier;
    public float playerUltMultiplier;
    public float playerMeleeResistance;
    public float playerRangedResistance;
    public float playerSpeedMultiplier;
    public float playerUltCooldownMultiplier;
    public int regenerateHealth = 0;
    public int revengeNumber = 0;
    [Header("Inventory variables")]
    public bool inventoryIsOpened;
    [SerializeField] string[] equipedRunes;
    [Header("InventroyUI")]
    public GameObject runeCanvas;
    [SerializeField] GameObject baseUI;
    public GameObject[] runeSlots;
    public int[] runesInSlots;
    public Image pickedUpRuneSlot;
    public TextMeshProUGUI pickedUpRuneDescText;
    public Sprite defaultSprite;
    public Sprite emptySprite;
    [Header("Weapons UI")]
    [SerializeField] GameObject weaponsUI;
    public Vector3 weaponsUIPosition;
    [Header("Other stuff")]
    [SerializeField] GameObject wave;

    private GameObject pickedUpRune;
    private Vector3 originalPosition;
    public Canvas canvas;

    private GameObject toBeDeleted;
    private bool isDragging;
    private PlayerHealth healthSc;
    private Sounds soundScript;

    public bool isFroozen = false;

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
        runesInSlots = new int[] { 0, 0, 0, 0, 0};
        healthSc = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (movementActions.PlayerMap.Runes.triggered && !isFroozen)
        {
            if (!inventoryIsOpened && Time.timeScale != 0)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }

        // Handling touch input for dragging
        if (isDragging && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touchPosition, canvas.worldCamera, out touchPosition);
            pickedUpRuneSlot.rectTransform.anchoredPosition = touchPosition;
        }

        // Start dragging when a touch begins on the UI slot (used for inventory)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && IsTouchOverImage() && inventoryIsOpened)
        {
            isDragging = true;
        }

        // Stop dragging when touch ends
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && isDragging)
        {
            isDragging = false;
            DetectUIObjectUnderTouch(true);
        }

        // Detect when the touch is released without dragging
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && inventoryIsOpened)
        {
            DetectUIObjectUnderTouch(false);
        }
    }

    void ActivateRune(string runeName)
    {
        soundScript.MakeSound("inventoryChangeSound", 0.5f);
        switch(runeName)
        {
            case "Speed":
                playerSpeedMultiplier *= 2;
                break;
            case "Cooldown":
                playerUltCooldownMultiplier /= 2;
                break;
            case "Power":
                playerMeleeMultiplier *= 1.5f;
                playerRangedMultiplier /= 2;
                break;
            case "Magic":
                playerMeleeMultiplier /= 2;
                playerRangedMultiplier *= 1.5f;
                break;
            case "Regeneration":
                regenerateHealth +=15;
                break;
            case "Shield":
                playerRangedResistance/=2;
                playerRangedMultiplier /= 2;
                break;
            case "Bubble":
                playerMeleeResistance /= 2;
                playerMeleeMultiplier /= 2;
                break;
            case "Skull":
                revengeNumber++;
                break;
        }
    }

    void DropRune(string runeName, bool isSwapping, GameObject runeSlot, int slotIndex)
    {
        soundScript.MakeSound("inventoryChangeSound", 0.5f);
        runesInSlots[slotIndex]--;
        switch (runeName)
        {
            case "Speed":
                playerSpeedMultiplier /= 2;
                break;
            case "Cooldown":
                playerUltCooldownMultiplier *= 2;
                break;
            case "Power":
                playerMeleeMultiplier /= 1.5f;
                playerRangedMultiplier *= 2;
                break;
            case "Magic":
                playerMeleeMultiplier *= 2;
                playerRangedMultiplier /= 1.5f;
                break;
            case "Regeneration":
                regenerateHealth -= 15;
                break;
            case "Shield":
                playerRangedResistance *= 2;
                playerRangedMultiplier *= 2;
                break;
            case "Bubble":
                playerMeleeResistance *= 2;
                playerMeleeMultiplier *= 2;
                break;
            case "Skull":
                revengeNumber--;
                break;
        }
        LoadPrefabByName(runeName + "_PickUp", true);
        if (!isSwapping && runesInSlots[slotIndex]==0)
        {
            runeSlot.GetComponent<Image>().sprite = defaultSprite;
            runeSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
            runeSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            runesInSlots[slotIndex] = 0;
        }
        else if(isSwapping)
        {
            runesInSlots[slotIndex] = 0;
        }
        else
        {
            runeSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + runesInSlots[slotIndex];
        }
    }

    public void PickUpRune(GameObject rune)
    {
        toBeDeleted = rune;
        string runeName = rune.name.Split('_')[0];
        OpenInventory();
        LoadPrefabByName(runeName + "_Icon", false);
        switch (runeName)
        {
            case "Speed":
                pickedUpRuneDescText.text = "Speed: Increases player speed by 100%";
                break;
            case "Cooldown":
                pickedUpRuneDescText.text = "Cooldown: Decreases weapon's ultimate cooldown by 50%";
                break;
            case "Power":
                pickedUpRuneDescText.text = "Power: Increases melee damage by 50%, decreases ranged damage by 50%";
                break;
            case "Magic":
                pickedUpRuneDescText.text = "Magic: Increases ranged damage by 50%, decreases melee damage by 50%";
                break;
            case "Regeneration":
                pickedUpRuneDescText.text = "Regeneration: Clearing a room regenerates 15 health points";
                break;
            case "Shield":
                pickedUpRuneDescText.text = "Shield: Decreases ranged damage taken by 50%, decreases ranged damage by 50%";
                break;
            case "Bubble":
                pickedUpRuneDescText.text = "Bubble: Decreases melee damage taken by 50%, decreases melee damage by 50%";
                break;
            case "Skull":
                pickedUpRuneDescText.text = "Skull: Any time you take damage you create a shockwave arround you";
                break;
        }
    }

    public void EquipRune(GameObject runeSlot, int slotIndex)
    {
        Destroy(toBeDeleted);
        string runeName = runeSlot.GetComponentInChildren<TextMeshProUGUI>().text;
        if (runeName.Length!=0)
        {
            if (runeName == pickedUpRuneDescText.text)
            {
                runesInSlots[slotIndex]++;
                runeSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "X" + runesInSlots[slotIndex];
                ActivateRune(pickedUpRuneDescText.text.Split(':')[0]);
                pickedUpRuneSlot.sprite = emptySprite;
                pickedUpRune = null;
                pickedUpRuneDescText.text = "";
            }
            else
            {
                int runesInSlot = runesInSlots[slotIndex];
                for (int i = 0; i<runesInSlot; i++)
                {
                    DropRune(runeName.Split(':')[0], true, runeSlot, slotIndex);
                }
                runesInSlots[slotIndex]++;
                runeSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "X" + runesInSlots[slotIndex];
                runeSlot.GetComponentInChildren<TextMeshProUGUI>().text = pickedUpRuneDescText.text;
                runeSlot.GetComponent<Image>().sprite = pickedUpRune.GetComponent<SpriteRenderer>().sprite;
                ActivateRune(pickedUpRuneDescText.text.Split(':')[0]);
                pickedUpRuneSlot.sprite = emptySprite;
                pickedUpRune = null;
                pickedUpRuneDescText.text = "";
            }
        }
        else
        {
            runesInSlots[slotIndex]++;
            runeSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "X" + runesInSlots[slotIndex];
            runeSlot.GetComponentInChildren<TextMeshProUGUI>().text = pickedUpRuneDescText.text;
            runeSlot.GetComponent<Image>().sprite = pickedUpRune.GetComponent<SpriteRenderer>().sprite;
            ActivateRune(pickedUpRuneDescText.text.Split(':')[0]);
            pickedUpRuneSlot.sprite = emptySprite;
            pickedUpRune = null;
            pickedUpRuneDescText.text = "";
        }
    }

    void LoadPrefabByName(string prefabName, bool isDropping)
    {
        Addressables.LoadAssetAsync<GameObject>(prefabName).Completed += (obj) => OnPrefabLoaded(obj, isDropping);
    }

    void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj, bool isDropping)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            if (!isDropping)
            {
                GameObject prefab = obj.Result;
                pickedUpRune = prefab;
                pickedUpRuneSlot.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                GameObject prefab = obj.Result;
                Instantiate(prefab, transform.position, prefab.transform.rotation);
            }   
        }
        else
        {
            Debug.LogError("Can't load prefab: " + obj.OperationException);
        }
    }

    private void DetectUIObjectUnderTouch(bool isDropped)
    {
        if (isDropped)
        {
            for (int i = 0; i < runeSlots.Length; i++)
            {
                if (IsTouchOverSlot(i))
                {
                    EquipRune(runeSlots[i], i);
                }
                else
                {
                    pickedUpRuneSlot.transform.position = originalPosition;
                }
            }
        }
        else
        {
            for (int i = 0; i < runeSlots.Length; i++)
            {
                if (IsTouchOverSlot(i))
                {
                    string runeName = runeSlots[i].GetComponentInChildren<TextMeshProUGUI>().text;
                    if (runeName.Length != 0)
                    {
                        DropRune(runeName.Split(':')[0], false, runeSlots[i], i);
                    }
                }
            }
        }
    }

    private bool IsTouchOverSlot(int slotIndex)
    {
        RectTransform slotRect = runeSlots[slotIndex].GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.GetTouch(0).position);
    }

    private bool IsTouchOverImage()
    {
        RectTransform slotRect = pickedUpRuneSlot.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.GetTouch(0).position);
    }

    void OpenInventory()
    {
        inventoryIsOpened = true;

        runeCanvas.SetActive(true);
        baseUI.SetActive(false);

        Time.timeScale = 0;
        weaponsUI.transform.position = new Vector3(100, 100, 0);

        originalPosition = pickedUpRuneSlot.transform.position;
    }

    public void CloseInventory()
    {
        runeCanvas.SetActive(false);
        baseUI.SetActive(true);

        inventoryIsOpened = false;
        pickedUpRuneSlot.sprite = emptySprite;
        pickedUpRune = null;
        pickedUpRuneDescText.text = "";
        toBeDeleted = null;

        Time.timeScale = 1;
        weaponsUI.transform.localPosition = weaponsUIPosition;
    }

    public void NewRoom()
    {
        if (healthSc.healhPoints + regenerateHealth >= 100)
        {
            healthSc.healhPoints = 100;
        }
        else
        {
            healthSc.healhPoints += regenerateHealth;
        }
    }

    public IEnumerator TakesDamage()
    {
        for (int i = 0; i < revengeNumber; i++)
        {
            soundScript.MakeSound("hammerSound", 0.5f);
            Instantiate(wave, transform.position, transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
