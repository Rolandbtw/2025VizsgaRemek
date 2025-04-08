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
    [SerializeField] Slider soundsSlider;
    [SerializeField] Slider soundsSlider2;
    [SerializeField] AudioMixer soundsMixer;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Toggle aimAssistToggle;

    private void Start()
    {
        soundsSlider.value = PlayerPrefs.GetFloat("sounds");
        soundsSlider2.value = PlayerPrefs.GetFloat("music");
        soundsMixer.SetFloat("Volume", PlayerPrefs.GetFloat("sounds"));
        musicMixer.SetFloat("Music", PlayerPrefs.GetFloat("music"));

        if (!PlayerPrefs.HasKey("AimAssist"))
        {
            PlayerPrefs.SetInt("AimAssist", 0);
        }
        if (PlayerPrefs.GetInt("AimAssist") == 0)
        {
            aimAssistToggle.isOn = false;
        }
        else if(PlayerPrefs.GetInt("AimAssist")==1)
        {
            aimAssistToggle.isOn = true;
        }
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

    public void ChangeAimAssist()
    {
        if (PlayerPrefs.GetInt("AimAssist") == 0)
        {
            PlayerPrefs.SetInt("AimAssist", 1);
        }
        else if (PlayerPrefs.GetInt("AimAssist") == 1)
        {
            PlayerPrefs.SetInt("AimAssist", 0);
        }
    }
}
