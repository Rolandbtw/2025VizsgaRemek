using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BombAnim : MonoBehaviour
{
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    private SpriteRenderer bombRenderer;
    [SerializeField] Color yellow;
    [SerializeField] Color red;
    private Light2D lightScript;

    private void Start()
    {
        lightScript = GetComponentInChildren<Light2D>();
        lightScript.enabled = false;
        bombRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator ExplosionAnim(float waittime)
    {
        lightScript.enabled = true;
        lightScript.color = yellow;
        bombRenderer.sprite = sprite1;
        yield return new WaitForSeconds(waittime);
        bombRenderer.sprite = sprite2;
        lightScript.color = red;
    }
}
