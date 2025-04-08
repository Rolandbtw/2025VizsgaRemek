using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public int coins = 0;
    [SerializeField] TextMeshProUGUI coinText;

    private void Update()
    {
        coinText.text = "x " + coins;
    }
}
