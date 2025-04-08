using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] Slider soundsSlider;
    [SerializeField] Slider soundsSlider2;
    [SerializeField] AudioMixer soundsMixer;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] TextMeshProUGUI bindingText;
    private Button button;
    private bool isSetting=false;
    private float timer;
    private string originalText;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            if (PlayerPrefs.HasKey(button.name))
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString(button.name);
            }
            else
            {
                PlayerPrefs.SetString(button.name, button.GetComponentInChildren<TextMeshProUGUI>().text);
            }
        }

        soundsSlider.value = PlayerPrefs.GetFloat("sounds");
        soundsSlider2.value = PlayerPrefs.GetFloat("music");
        soundsMixer.SetFloat("Volume", PlayerPrefs.GetFloat("sounds"));
        musicMixer.SetFloat("Music", PlayerPrefs.GetFloat("music"));
    }

    public void SoundSliderOnChange(float volume)
    {
        PlayerPrefs.SetFloat("sounds", volume);
        soundsMixer.SetFloat("Volume", volume);
    }

    public void MusicSliderOnChange(float volume)
    {
        PlayerPrefs.SetFloat("music", volume);
        musicMixer.SetFloat("Music", volume);
    }

    private void Update()
    {
        if (isSetting)
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    if (KeyBindings.IsAlreadyBinded(keycode.ToString()))
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = originalText;
                        StartCoroutine(ShowMessage("Button already binded."));
                        isSetting = false;
                        timer = Time.time + 0.1f;
                    }
                    else
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = keycode.ToString();
                        PlayerPrefs.SetString(button.name, keycode.ToString());
                        PlayerPrefs.Save();
                        isSetting = false;
                        timer = Time.time + 0.1f;
                    }
                    Cursor.visible = true;
                }
            }
        }
    }

    public void KeyBinding()
    {
        if (!isSetting && timer<Time.time)
        {
            button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            originalText = button.GetComponentInChildren<TextMeshProUGUI>().text;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "_";

            isSetting = true;
            Cursor.visible = false;
        }
    }

    IEnumerator ShowMessage(string message)
    {
        bindingText.text = message;
        yield return new WaitForSeconds(2);
        bindingText.text = "";
    }
}
