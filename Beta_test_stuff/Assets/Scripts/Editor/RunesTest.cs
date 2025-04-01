using NUnit.Framework;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class RunesTest : MonoBehaviour
{
    private GameObject runesObject;
    private Runes runes;
    private GameObject generator;
    private GameObject dummyRuneSlot;
    private MethodInfo activateRune;
    private MethodInfo dropRune;

    [SetUp]
    public void SetUp()
    {
        generator = new GameObject("Generator");
        generator.tag = "Generator";
        generator.AddComponent<Sounds>();

        runesObject = new GameObject();
        runes = runesObject.AddComponent<Runes>();

        MethodInfo startMethod = typeof(Runes).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(runes, null);

        activateRune = typeof(Runes).GetMethod("ActivateRune", BindingFlags.NonPublic | BindingFlags.Instance);
        dropRune = typeof(Runes).GetMethod("DropRune", BindingFlags.NonPublic | BindingFlags.Instance);

        dummyRuneSlot = new GameObject("DummyRuneSlot");
        dummyRuneSlot.AddComponent<Image>();

        var textChild = new GameObject("Text");
        textChild.AddComponent<TextMeshProUGUI>();
        textChild.transform.SetParent(dummyRuneSlot.transform);

        var countTextChild = new GameObject("CountText");
        countTextChild.AddComponent<TextMeshProUGUI>();
        countTextChild.transform.SetParent(dummyRuneSlot.transform);

        FieldInfo field = typeof(Runes).GetField("runesInSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(runes, new int[] { 1, 0, 0, 0, 0 });
    }

    [Test]
    public void ActivateRune_Speed_DoublesSpeedMultiplier()
    {
        float initialSpeed = runes.playerSpeedMultiplier = 1f;

        object[] parameters = new object[] { "Speed" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialSpeed * 2, runes.playerSpeedMultiplier);
    }

    [Test]
    public void DropRune_Speed_HalvesSpeedMultiplier()
    {
        runes.playerSpeedMultiplier = 2f;

        object[] parameters = new object[] { "Speed", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerSpeedMultiplier);
    }

    [Test]
    public void ActivateRune_Cooldown_HalvesUltimateCooldownMultiplier()
    {
        float initialCooldown = runes.playerUltCooldownMultiplier = 1f;

        object[] parameters = new object[] { "Cooldown" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialCooldown / 2, runes.playerUltCooldownMultiplier);
    }

    [Test]
    public void DropRune_Cooldown_DoublesUltimateCooldownMultiplier()
    {
        runes.playerUltCooldownMultiplier = 0.5f;

        object[] parameters = new object[] { "Cooldown", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerUltCooldownMultiplier);
    }

    [Test]
    public void ActivateRune_Power_IncreasesMeleeMultiplierDecreasesRangedMultiplier()
    {
        float initialMelee = runes.playerMeleeMultiplier = 1f;
        float initialRanged = runes.playerRangedMultiplier = 1f;

        object[] parameters = new object[] { "Power" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialMelee * 1.5f, runes.playerMeleeMultiplier);
        Assert.AreEqual(initialRanged / 2, runes.playerRangedMultiplier);
    }

    [Test]
    public void DropRune_Power_DecreasesMeleeMultiplierIncreasesRangedMultiplier()
    {
        runes.playerMeleeMultiplier = 1.5f;
        runes.playerRangedMultiplier = 0.5f;

        object[] parameters = new object[] { "Power", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerMeleeMultiplier);
        Assert.AreEqual(1f, runes.playerRangedMultiplier);
    }

    [Test]
    public void ActivateRune_Magic_IncreasesRangedMultiplierDecreasesMeleeMultiplier()
    {
        float initialMelee = runes.playerMeleeMultiplier = 1f;
        float initialRanged = runes.playerRangedMultiplier = 1f;

        object[] parameters = new object[] { "Magic" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialMelee / 2, runes.playerMeleeMultiplier);
        Assert.AreEqual(initialRanged * 1.5f, runes.playerRangedMultiplier);
    }

    [Test]
    public void DropRune_Magic_DecreasesRangedMultiplierIncreasesMeleeMultiplier()
    {
        runes.playerMeleeMultiplier = 0.5f;
        runes.playerRangedMultiplier = 1.5f;

        object[] parameters = new object[] { "Magic", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerMeleeMultiplier);
        Assert.AreEqual(1f, runes.playerRangedMultiplier);
    }

    [Test]
    public void ActivateRune_Regeneration_IncreasesHealthRegeneration()
    {
        int initialHealth = runes.regenerateHealth = 0;

        object[] parameters = new object[] { "Regeneration" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialHealth + 15, runes.regenerateHealth);
    }

    [Test]
    public void DropRune_Regeneration_DecreasesHealthRegeneration()
    {
        runes.regenerateHealth = 15;

        object[] parameters = new object[] { "Regeneration", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(0, runes.regenerateHealth);
    }

    [Test]
    public void ActivateRune_Shield_DecreasesRangedResistance()
    {
        float initialResistance = runes.playerRangedResistance = 1f;

        object[] parameters = new object[] { "Shield" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialResistance / 2, runes.playerRangedResistance);
    }

    [Test]
    public void DropRune_Shield_IncreasesRangedResistance()
    {
        runes.playerRangedResistance = 0.5f;

        object[] parameters = new object[] { "Shield", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerRangedResistance);
    }

    [Test]
    public void ActivateRune_Bubble_DecreasesMeleeResistance()
    {
        float initialResistance = runes.playerMeleeResistance = 1f;

        object[] parameters = new object[] { "Bubble" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialResistance / 2, runes.playerMeleeResistance);
    }

    [Test]
    public void DropRune_Bubble_IncreasesMeleeResistance()
    {
        runes.playerMeleeResistance = 0.5f;

        object[] parameters = new object[] { "Bubble", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(1f, runes.playerMeleeResistance);
    }

    [Test]
    public void ActivateRune_Skull_IncreasesRevengeNumber()
    {
        int initialRevenge = runes.revengeNumber = 0;

        object[] parameters = new object[] { "Skull" };
        activateRune.Invoke(runes, parameters);

        Assert.AreEqual(initialRevenge + 1, runes.revengeNumber);
    }

    [Test]
    public void DropRune_Skull_DecreasesRevengeNumber()
    {
        runes.revengeNumber = 1;

        object[] parameters = new object[] { "Skull", false, dummyRuneSlot, 0 };
        dropRune.Invoke(runes, parameters);

        Assert.AreEqual(0, runes.revengeNumber);
    }
}
