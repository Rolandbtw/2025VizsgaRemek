using System;
using UnityEngine;
using UnityEngine.SceneManagement;

static class KeyBindings
{
    public static KeyCode left;
    public static KeyCode right;
    public static KeyCode up;
    public static KeyCode down;
    public static KeyCode dash;
    public static KeyCode interact;
    public static KeyCode change;
    public static KeyCode drop;
    public static KeyCode runes;
    public static KeyCode attack;
    public static KeyCode ultAttack;

    private static string[] keyNames = { "Left", "Right", "Up", "Down", "Dash", "Interact", "Switch", "Drop", "Runes", "Fire", "AltFire" };

    static KeyBindings()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadKeyBindings();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadKeyBindings();
    }

    static void LoadKeyBindings()
    {
        if (PlayerPrefs.HasKey("Left"))
        {
            left = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
            right = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
            up = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
            down = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
            dash = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Dash"));
            interact = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact"));
            change = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Switch"));
            drop = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Drop"));
            runes = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Runes"));
            attack = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire"));
            ultAttack = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AltFire"));
        }
        else
        {
            SetDefaultKeyBindings();
        }
    }

    static void SetDefaultKeyBindings()
    {
        left = KeyCode.A;
        PlayerPrefs.SetString("Left", "A");
        right = KeyCode.D;
        PlayerPrefs.SetString("Right", "D");
        up = KeyCode.W;
        PlayerPrefs.SetString("Up", "W");
        down = KeyCode.S;
        PlayerPrefs.SetString("Down", "S");
        dash = KeyCode.LeftShift;
        PlayerPrefs.SetString("Dash", "LeftShift");
        interact = KeyCode.F;
        PlayerPrefs.SetString("Interact", "F");
        change = KeyCode.R;
        PlayerPrefs.SetString("Switch", "R");
        drop = KeyCode.Q;
        PlayerPrefs.SetString("Drop", "Q");
        runes = KeyCode.Tab;
        PlayerPrefs.SetString("Runes", "Tab");
        attack = KeyCode.Mouse0;
        PlayerPrefs.SetString("Fire", "Mouse0");
        ultAttack = KeyCode.Mouse1;
        PlayerPrefs.SetString("AltFire", "Mouse1");
    }

    static public bool IsAlreadyBinded(string value)
    {
        foreach (var key in keyNames)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string keyValue = PlayerPrefs.GetString(key);
                if (keyValue == value)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
