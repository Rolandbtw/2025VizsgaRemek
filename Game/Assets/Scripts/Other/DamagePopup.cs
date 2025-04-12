using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        Destroy(gameObject, 0.45f);
    }

    public void SetDamageText(float damage)
    {
        damage=MathF.Round(damage, 2);
        text.text = damage.ToString();
    }
}
