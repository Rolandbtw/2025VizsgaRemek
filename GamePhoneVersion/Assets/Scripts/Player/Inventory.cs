using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject inventorySlot1;
    [SerializeField] GameObject inventorySlot2;
    [SerializeField] GameObject weaponPlace;

    [Header("Floats to customize")]
    [SerializeField] float throwForce;

    [Header("Variables for other scripts to reach")]
    public bool usingWeapon = false;
    public string currentWeapon = "";

    string secondaryWeapon = "";
    Vector3 throwAngle;
    Runes runes;
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
        runes = GetComponent<Runes>();
    }

    public void Update()
    {
        if (!runes.inventoryIsOpened && !GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>().isInTransition)
        {
            if (movementActions.PlayerMap.Switch.triggered && inventorySlot2.transform.childCount > 0 && !usingWeapon)
            {
                SwapWeapons();
                soundScript.MakeSound("inventoryChangeSound", 0.5f);
            }
            if (movementActions.PlayerMap.Drop.triggered && currentWeapon != "" && !usingWeapon)
            {
                ThrowWeapon();
                soundScript.MakeSound("inventoryChangeSound", 0.5f);
            }
        }

        Vector2 joystickVector = movementActions.PlayerMap.Movement.ReadValue<Vector2>();

        if (joystickVector != new Vector2(0, 0))
        {
            throwAngle = new Vector3(joystickVector.x, joystickVector.y, 0);
        }
    }

    public void PickUp(string weaponName)
    {
        soundScript.MakeSound("playerPickUpSound", 0.5f);
        if (currentWeapon == "" && secondaryWeapon == "" || currentWeapon == "" && secondaryWeapon != "")
        {
            currentWeapon = weaponName;
            LoadPrefabByName(currentWeapon, true);
            LoadPrefabByName(currentWeapon + "_Icon", true);
        }
        else if (currentWeapon != "" && secondaryWeapon == "")
        {
            secondaryWeapon = weaponName;
            LoadPrefabByName(secondaryWeapon + "_Icon", false);
        }
        else if (currentWeapon != "" && secondaryWeapon != "")
        {
            ThrowWeapon();
            currentWeapon = weaponName;
            LoadPrefabByName(currentWeapon, true);
            LoadPrefabByName(currentWeapon + "_Icon", true);
        }
    }

    void SwapWeapons()
    {
        string tempWeapon = currentWeapon;
        currentWeapon = secondaryWeapon;
        secondaryWeapon = tempWeapon;

        if (tempWeapon != "")
        {
            AdaptiveDestroy(inventorySlot1.transform.GetChild(0).gameObject);
            AdaptiveDestroy(weaponPlace.transform.GetChild(0).gameObject);
        }
        AdaptiveDestroy(inventorySlot2.transform.GetChild(0).gameObject);


        if (tempWeapon != "")
        {
            LoadPrefabByName(secondaryWeapon + "_Icon", false);
        }
        LoadPrefabByName(currentWeapon, true);
        LoadPrefabByName(currentWeapon + "_Icon", true);
    }

    void ThrowWeapon()
    {
        AdaptiveDestroy(inventorySlot1.transform.GetChild(0).gameObject);
        AdaptiveDestroy(weaponPlace.transform.GetChild(0).gameObject);
        LoadPrefabByName(currentWeapon + "_PickUp", false);
        currentWeapon = "";
    }

    void LoadPrefabByName(string prefabName, bool isForSlot1)
    {
        Addressables.LoadAssetAsync<GameObject>(prefabName).Completed += (obj) => OnPrefabLoaded(obj, isForSlot1);
    }

    void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj, bool isForSlot1)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded && obj.Result != null)
        {
            GameObject prefab = obj.Result;

            if (prefab.name.Contains("Icon"))
            {
                if (isForSlot1)
                {
                    Instantiate(prefab, inventorySlot1.transform.position, prefab.transform.rotation, inventorySlot1.transform);
                }
                else
                {
                    Instantiate(prefab, inventorySlot2.transform.position, prefab.transform.rotation, inventorySlot2.transform);
                }
            }
            else if (prefab.name.Contains("PickUp"))
            {
                GameObject weaponPickUp = Instantiate(prefab, weaponPlace.transform.position, prefab.transform.rotation);

                weaponPickUp.GetComponent<Rigidbody2D>().AddForce(throwAngle * throwForce, ForceMode2D.Impulse);

            }
            else
            {
                Instantiate(prefab, weaponPlace.transform.position, transform.rotation, weaponPlace.transform);
            }
        }
        else
        {
            Debug.LogError("Can't load prefab: " + obj.OperationException);
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
