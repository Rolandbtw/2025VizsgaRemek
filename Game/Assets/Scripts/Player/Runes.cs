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
    [Header("Modifiers")]
    public float playerMeleeMultiplier;
    public float playerRangedMultiplier;
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
    public GameObject[] runeSlots;
    private int[] runesInSlots=new int[] { 0, 0, 0, 0, 0};
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

    private GameObject runePickUp;
    private bool isDragging;
    private PlayerHealth healthSc;
    private Sounds soundScript;

    public bool isFroozen = false;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        healthSc = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyBindings.runes) && !isFroozen)
        {
            if (!inventoryIsOpened && Time.timeScale!=0)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }

        if (isDragging)
        {
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePosition);
            pickedUpRuneSlot.rectTransform.anchoredPosition = mousePosition;
        }

        if (Input.GetMouseButtonDown(0) && IsMouseOverPickedUpRune() && inventoryIsOpened)
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            DetectUIObjectUnderMouse(true);
        }

        if (Input.GetMouseButtonDown(0) && inventoryIsOpened)
        {
            DetectUIObjectUnderMouse(false);
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

    void SpawnDroppedRune(string runeName)
    {
        LoadPrefabByName(runeName + "_PickUp", true);
    }

    public void PickUpRune(GameObject rune)
    {
        runePickUp = rune;
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
        AdaptiveDestroy(runePickUp);

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
                DropRune(runeName.Split(':')[0], true, runeSlot, slotIndex);
                SpawnDroppedRune(runeName.Split(':')[0]);

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
        if (obj.Status == AsyncOperationStatus.Succeeded || obj.Result != null)
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

    private bool IsMouseOverPickedUpRune()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == pickedUpRuneSlot.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void DetectUIObjectUnderMouse(bool isPickingUp)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        if (isPickingUp)
        {
            int count = 0;
            bool canCountinue = true;
            while(canCountinue && count < results.Count)
            {
                var result = results[count];
                if (result.gameObject != pickedUpRuneSlot.gameObject)
                {
                    if (result.gameObject.name.Contains("RuneSlot"))
                    {
                        string name = result.gameObject.name;
                        switch (name)
                        {
                            case "RuneSlot1":
                                EquipRune(runeSlots[0], 0);
                                break;
                            case "RuneSlot2":
                                EquipRune(runeSlots[1], 1);
                                break;
                            case "RuneSlot3":
                                EquipRune(runeSlots[2], 2);
                                break;
                            case "RuneSlot4":
                                EquipRune(runeSlots[3], 3);
                                break;
                            case "RuneSlot5":
                                EquipRune(runeSlots[4], 4);
                                break;
                        }
                    }
                    canCountinue=false;
                }
                count++;
            }
            pickedUpRuneSlot.transform.position = originalPosition;
        }
        else
        {
            int count = 0;
            bool canCountine = true;
            while(canCountine && count < results.Count)
            {
                var result = results[count];
                if (result.gameObject.name.Contains("RuneSlot"))
                {
                    string name = result.gameObject.name;
                    switch (name)
                    {
                        case "RuneSlot1":
                            string runeName = runeSlots[0].GetComponentInChildren<TextMeshProUGUI>().text;
                            if (runeName.Length != 0)
                            {
                                DropRune(runeName.Split(':')[0], false, runeSlots[0], 0);
                                SpawnDroppedRune(runeName.Split(':')[0]);
                            }
                            break;
                        case "RuneSlot2":
                            runeName = runeSlots[1].GetComponentInChildren<TextMeshProUGUI>().text;
                            if (runeName.Length != 0)
                            {
                                DropRune(runeName.Split(':')[0], false, runeSlots[1], 1);
                                SpawnDroppedRune(runeName.Split(':')[0]);
                            }
                            break;
                        case "RuneSlot3":
                            runeName = runeSlots[2].GetComponentInChildren<TextMeshProUGUI>().text;
                            if (runeName.Length != 0)
                            {
                                DropRune(runeName.Split(':')[0], false, runeSlots[2], 2);
                                SpawnDroppedRune(runeName.Split(':')[0]);
                            }
                            break;
                        case "RuneSlot4":
                            runeName = runeSlots[3].GetComponentInChildren<TextMeshProUGUI>().text;
                            if (runeName.Length != 0)
                            {
                                DropRune(runeName.Split(':')[0], false, runeSlots[3], 3);
                                SpawnDroppedRune(runeName.Split(':')[0]);
                            }
                            break;
                        case "RuneSlot5":
                            runeName = runeSlots[4].GetComponentInChildren<TextMeshProUGUI>().text;
                            if (runeName.Length != 0)
                            {
                                DropRune(runeName.Split(':')[0], false, runeSlots[4], 4);
                                SpawnDroppedRune(runeName.Split(':')[0]);
                            }
                            break;
                    }
                    canCountine = false;
                }
                count++;
            }
        }
    }

    void OpenInventory()
    {
        inventoryIsOpened = true;

        runeCanvas.SetActive(true);

        Time.timeScale = 0;
        weaponsUI.transform.position = new Vector3(100, 100, 0);

        originalPosition = pickedUpRuneSlot.transform.position;
    }

    public void CloseInventory()
    {
        runeCanvas.SetActive(false);
        inventoryIsOpened = false;
        pickedUpRuneSlot.sprite = emptySprite;
        pickedUpRune = null;
        pickedUpRuneDescText.text = "";
        runePickUp = null;

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

    void AdaptiveDestroy(GameObject toBeDeleted)
    {
        #if UNITY_EDITOR
            if (Application.isEditor) DestroyImmediate(toBeDeleted);
            else Destroy(toBeDeleted);
        #else
            Destroy(toBeDeleted);
        #endif
    }
}
