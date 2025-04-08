using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

public class Sounds : MonoBehaviour
{
    [Header("AudioMixers")]
    [SerializeField] AudioMixerGroup soundMixer;

    [Header("Player sounds")]
    [SerializeField] AudioClip playerDashSound;
    [SerializeField] AudioClip playerHurtSound;
    [SerializeField] AudioClip playerPickUpSound;
    [SerializeField] AudioClip playerPickUpSound2;
    [SerializeField] AudioClip inventoryChangeSound;
    [SerializeField] AudioClip playerHealSound;

    [Header("Weapon sounds")]
    [SerializeField] AudioClip slicingSound;
    [SerializeField] AudioClip hammerSound;
    [SerializeField] AudioClip swordSound;
    [SerializeField] AudioClip spearSound;
    [SerializeField] AudioClip staffSound;
    [SerializeField] AudioClip bowSound;
    [SerializeField] AudioClip meteorSpawnSound;
    [SerializeField] AudioClip meteorSound;

    [Header("Enemy sounds")]
    [SerializeField] AudioClip enemyDamageSound;
    [SerializeField] AudioClip enemyDeathSound;
    [SerializeField] AudioClip bombSound;
    [SerializeField] AudioClip goblinSound;
    [SerializeField] AudioClip summonEnemiesSound;
    [SerializeField] AudioClip bombThrowSound;
    [SerializeField] AudioClip demonDashSound;
    [SerializeField] AudioClip wizardShootSound;

    [Header("Other sounds")]
    [SerializeField] AudioClip chestSound;
    [SerializeField] AudioClip parrySound;
    [SerializeField] AudioClip iceSound;
    [SerializeField] AudioClip portalSpawnSound;
    [SerializeField] AudioClip portalSound;
    [SerializeField] AudioClip coinSound;
    [SerializeField] AudioClip buySound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioClip columnBreak;

    [Header("Music")]
    [SerializeField] AudioClip shop;
    [SerializeField] AudioClip battle;
    [SerializeField] AudioSource musicSource;

    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    private float cooldownTime = 0.1f;

    public void MakeSound(string clipName, float volume)
    {
        if (soundCooldowns.ContainsKey(clipName) && Time.time - soundCooldowns[clipName] < cooldownTime)
        {
            return;
        }
        soundCooldowns[clipName] = Time.time;

        FieldInfo fieldInfo = this.GetType().GetField(clipName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo != null && fieldInfo.FieldType == typeof(AudioClip))
        {
            AudioClip clip = (AudioClip)fieldInfo.GetValue(this);

            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = soundMixer;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            StartCoroutine(DestoryTempAudio(tempAudio, clip.length));
        }
    }

    IEnumerator DestoryTempAudio(GameObject tempAudio, float length)
    {
        yield return new WaitForSeconds(length);
        DestroyImmediate(tempAudio);
    }

    public void PlayMusic(int num)
    {
        switch (num)
        {
            case 0:
                musicSource.clip = shop;
                musicSource.Play();
                break;
            case 1:
                musicSource.clip = battle;
                musicSource.Play();
                break;
        }
    }
}
