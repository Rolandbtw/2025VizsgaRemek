using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RunesUITest : MonoBehaviour
{
    private GameObject runesObject;
    private Runes runes;
    private GameObject[] dummySlots;
    private GameObject dummyCanvas;
    private GameObject pickedUpRune;

    [SetUp]
    public void SetUp()
    {
        Camera camera = new GameObject("Camera").AddComponent<Camera>();
        camera.tag = "MainCamera";

        dummyCanvas = new GameObject("Canvas");
        dummyCanvas.AddComponent<Canvas>();
        dummyCanvas.AddComponent<CanvasScaler>();
        dummyCanvas.AddComponent<GraphicRaycaster>();
        dummyCanvas.AddComponent<EventSystem>();

        runesObject = new GameObject();
        runes = runesObject.AddComponent<Runes>();
        MethodInfo startMethod = typeof(Runes).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(runes, null);

        dummySlots = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            dummySlots[i] = CreateDummySlot($"RuneSlot{i + 1}");
        }

        pickedUpRune = new GameObject("Speed_PickUp");
        pickedUpRune.AddComponent<SpriteRenderer>().sprite = Sprite.Create(
            new Texture2D(1, 1),
            new Rect(0, 0, 1, 1),
            Vector2.zero
        );

        FieldInfo pickedUpRuneField = typeof(Runes).GetField("pickedUpRune", BindingFlags.NonPublic | BindingFlags.Instance);
        pickedUpRuneField.SetValue(runes, pickedUpRune);

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(runes, new int[] { 0, 0, 0, 0, 0 });
        runes.runeSlots = dummySlots;

        runes.pickedUpRuneDescText = new GameObject("DescText").AddComponent<TextMeshProUGUI>();
        runes.pickedUpRuneSlot = new GameObject("PickedUpSlot").AddComponent<Image>();
        runes.pickedUpRuneSlot.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
    }

    [Test]
    public void EquipRune_EmptySlot()
    {
        int slotIndex = 0;
        runes.pickedUpRuneDescText.text = "Speed: Description";
        var originalSpeed = runes.playerSpeedMultiplier;
        runes.EquipRune(dummySlots[slotIndex], slotIndex);

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] runesInSlots = (int[])field.GetValue(runes);

        Assert.AreEqual(1, runesInSlots[slotIndex]);
        Assert.AreEqual("Speed: Description", GetSlotText(slotIndex));
        Assert.AreEqual("X1", GetCountText(slotIndex));
        Assert.AreEqual(originalSpeed * 2, runes.playerSpeedMultiplier);
        Assert.AreEqual(pickedUpRune.GetComponent<SpriteRenderer>().sprite, dummySlots[slotIndex].GetComponent<Image>().sprite);
    }

    [Test]
    public void EquipRune_OccupiedSlot()
    {
        int slotIndex = 0;
        runes.pickedUpRuneDescText.text = "Speed: Description";
        var originalSpeed = runes.playerSpeedMultiplier;
        runes.EquipRune(dummySlots[slotIndex], slotIndex);

        FieldInfo weaponField = typeof(Runes).GetField("pickedUpRune", BindingFlags.NonPublic | BindingFlags.Instance);
        GameObject rune = new GameObject("Magic_PickUp");
        rune.AddComponent<SpriteRenderer>();
        weaponField.SetValue(runes, rune);

        runes.pickedUpRuneDescText.text = "Magic: Description";
        var originalRanged = runes.playerRangedMultiplier;
        runes.EquipRune(dummySlots[slotIndex], slotIndex);

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] runesInSlots = (int[])field.GetValue(runes);

        Assert.AreEqual(1, runesInSlots[slotIndex]);
        Assert.AreEqual("Magic: Description", GetSlotText(slotIndex));
        Assert.AreEqual(originalSpeed / 2, runes.playerSpeedMultiplier);
        Assert.AreEqual(originalRanged * 1.5f, runes.playerRangedMultiplier);
    }

    [Test]
    public void EquipRune_Stacking()
    {
        int slotIndex = 0;
        runes.pickedUpRuneDescText.text = "Speed: Description";
        var originalSpeed = runes.playerSpeedMultiplier;

        runes.EquipRune(dummySlots[slotIndex], slotIndex);
        var speedAfterFirstEquip = runes.playerSpeedMultiplier;

        runes.pickedUpRuneDescText.text = "Speed: Description";
        runes.EquipRune(dummySlots[slotIndex], slotIndex);

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] runesInSlots = (int[])field.GetValue(runes);

        Assert.AreEqual(2, runesInSlots[slotIndex]);
        Assert.AreEqual("Speed: Description", GetSlotText(slotIndex));
        Assert.AreEqual("X2", GetCountText(slotIndex)); // We expect X2 as the count
        Assert.AreEqual(speedAfterFirstEquip * 2, runes.playerSpeedMultiplier);
    }

    [Test]
    public void DropRune_NonStacked()
    {
        int slotIndex = 0;
        runes.pickedUpRuneDescText.text = "Speed: Description";
        var originalSpeed = runes.playerSpeedMultiplier;

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] runesInSlots = (int[])field.GetValue(runes);

        runes.EquipRune(dummySlots[slotIndex], slotIndex);
        Assert.AreEqual(1, runesInSlots[slotIndex]);

        MethodInfo dropMethod = typeof(Runes).GetMethod("DropRune", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] {"Speed", false, dummySlots[slotIndex], slotIndex };
        dropMethod.Invoke(runes, parameters);

        field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        runesInSlots = (int[])field.GetValue(runes);

        Assert.AreEqual(0, runesInSlots[slotIndex]);
        Assert.AreEqual("", GetSlotText(slotIndex));
        Assert.AreEqual("", GetCountText(slotIndex));
        Assert.AreEqual(originalSpeed, runes.playerSpeedMultiplier);
    }

    [Test]
    public void DropRune_Stacked()
    {
        int slotIndex = 0;
        runes.pickedUpRuneDescText.text = "Speed: Description";
        var originalSpeed = runes.playerSpeedMultiplier;

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] runesInSlots = (int[])field.GetValue(runes);

        runes.EquipRune(dummySlots[slotIndex], slotIndex);
        Assert.AreEqual(1, runesInSlots[slotIndex]);

        field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        runesInSlots = (int[])field.GetValue(runes);

        runes.pickedUpRuneDescText.text = "Speed: Description";
        runes.EquipRune(dummySlots[slotIndex], slotIndex);
        Assert.AreEqual(2, runesInSlots[slotIndex]);

        MethodInfo dropMethod = typeof(Runes).GetMethod("DropRune", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] { "Speed", false, dummySlots[slotIndex], slotIndex };
        dropMethod.Invoke(runes, parameters);

        field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        runesInSlots = (int[])field.GetValue(runes);

        Assert.AreEqual(1, runesInSlots[slotIndex]);
        Assert.AreEqual("Speed: Description", GetSlotText(slotIndex));
        Assert.AreEqual("x1", GetCountText(slotIndex));
        Assert.AreEqual(originalSpeed * 2, runes.playerSpeedMultiplier);
    }

    private GameObject CreateDummySlot(string name)
    {
        GameObject slot = new GameObject(name);
        slot.AddComponent<Image>();

        GameObject nameText = new GameObject("Text");
        nameText.transform.SetParent(slot.transform);
        var nameComponent = nameText.AddComponent<TextMeshProUGUI>();
        nameComponent.text = "";

        GameObject countText = new GameObject("CountText");
        countText.transform.SetParent(slot.transform);
        var countComponent = countText.AddComponent<TextMeshProUGUI>();
        countComponent.text = "";

        return slot;
    }

    private string GetSlotText(int slotIndex)
    {
        return dummySlots[slotIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }

    private string GetCountText(int slotIndex)
    {
        return dummySlots[slotIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
    }
}
