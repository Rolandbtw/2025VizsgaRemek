using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BombAnim : MonoBehaviour
{
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    private SpriteRenderer bombRenderer;

    private void Start()
    {
        bombRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator ExplosionAnim(float waittime)
    {
        bombRenderer.sprite = sprite1;
        yield return new WaitForSeconds(waittime);
        bombRenderer.sprite = sprite2;
    }
}
