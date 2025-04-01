using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InventoryTest
{
    private GameObject player;
    private Inventory inventory;
    private GameObject generator;
    private GameObject inventorySlot1;
    private GameObject inventorySlot2;
    private GameObject weaponPlace;

    [SetUp]
    public void SetUp()
    {
        generator = new GameObject("Generator");
        generator.tag = "Generator";
        generator.AddComponent<Sounds>();

        inventorySlot1 = new GameObject("InventorySlot");
        inventorySlot1.transform.position = new Vector3(0, 0, 0);
        inventorySlot2 = new GameObject("InventorySlot2");
        inventorySlot2.transform.position = new Vector3(0, 0, 0);
        weaponPlace = new GameObject("weaponPlace");
        weaponPlace.transform.position = new Vector3(0, 0, 0);

        player = new GameObject("Player");
        player.AddComponent<Inventory>();
        inventory = player.GetComponent<Inventory>();

        SetFieldValue("inventorySlot1", inventorySlot1);
        SetFieldValue("inventorySlot2", inventorySlot2);
        SetFieldValue("weaponPlace", weaponPlace);

        MethodInfo startMethod = typeof(Inventory).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(inventory, null);

        inventory.currentWeapon = "";
        SetFieldValue("secondaryWeapon", "");

        Addressables.LoadAssetAsync<GameObject>("Sword").WaitForCompletion();
        Addressables.InitializeAsync().WaitForCompletion();

        Addressables.LoadAssetAsync<GameObject>("Sword_PickUp").WaitForCompletion();
        Addressables.InitializeAsync().WaitForCompletion();

        Addressables.LoadAssetAsync<GameObject>("Sword_Icon").WaitForCompletion();
        Addressables.InitializeAsync().WaitForCompletion();

        Addressables.LoadAssetAsync<GameObject>("Hammer").WaitForCompletion();
        Addressables.InitializeAsync().WaitForCompletion();

        Addressables.LoadAssetAsync<GameObject>("Hammer_Icon").WaitForCompletion();
        Addressables.InitializeAsync().WaitForCompletion();
    }

    [UnityTest]
    public IEnumerator PickUpWeaponTest_EmptyInventory()
    {
        inventory.PickUp("Sword");

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>("Sword");
        yield return loadHandle;

        GameObject weapon = GameObject.FindGameObjectWithTag("Weapon");

        Assert.NotNull(weapon);
        Assert.AreEqual("Sword", inventory.currentWeapon);
    }

    [UnityTest]
    public IEnumerator PickUpWeaponTest_NotEmptyInventory()
    {
        inventory.PickUp("Sword");

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>("Sword");
        yield return loadHandle;

        inventory.PickUp("Hammer");

        loadHandle = Addressables.LoadAssetAsync<GameObject>("Hammer_Icon");
        yield return loadHandle;

        GameObject icon = GameObject.Find("Hammer_Icon(Clone)");

        Assert.AreEqual(inventory.currentWeapon, "Sword");
        Assert.AreEqual((string)GetFieldValue("secondaryWeapon"), "Hammer");
        Assert.NotNull(icon);
    }

    [UnityTest]
    public IEnumerator SwapWeaponTest()
    {
        inventory.PickUp("Sword");

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>("Sword");
        yield return loadHandle;

        inventory.PickUp("Hammer");

        loadHandle = Addressables.LoadAssetAsync<GameObject>("Hammer_Icon");
        yield return loadHandle;

        MethodInfo swapMethod = typeof(Inventory).GetMethod("SwapWeapons", BindingFlags.NonPublic | BindingFlags.Instance);
        swapMethod.Invoke(inventory, null);

        loadHandle = Addressables.LoadAssetAsync<GameObject>("Hammer");
        yield return loadHandle;

        GameObject hammer = GameObject.Find("Hammer(Clone)");

        Assert.AreEqual(inventory.currentWeapon, "Hammer");
        Assert.AreEqual((string)GetFieldValue("secondaryWeapon"), "Sword");
        Assert.NotNull(hammer);
    }

    [UnityTest]
    public IEnumerator DropWeaponTest()
    {
        inventory.PickUp("Sword");

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>("Sword");
        yield return loadHandle;

        MethodInfo throwMethod = typeof(Inventory).GetMethod("ThrowWeapon", BindingFlags.NonPublic | BindingFlags.Instance);
        throwMethod.Invoke(inventory, null);

        loadHandle = Addressables.LoadAssetAsync<GameObject>("Sword_PickUp");
        yield return loadHandle;

        GameObject weapon= GameObject.FindGameObjectWithTag("Weapon");
        GameObject weaponPickUp = GameObject.Find("Sword_PickUp(Clone)");

        Assert.AreEqual(inventory.currentWeapon, "");
        Assert.IsNull(weapon);
        Assert.NotNull(weaponPickUp);
    }

    private void SetFieldValue(string fieldName, object value)
    {
        FieldInfo field = typeof(Inventory).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(inventory, value);
    }

    private object GetFieldValue(string fieldName)
    {
        FieldInfo field = typeof(Inventory).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field.GetValue(inventory);
    }
}
