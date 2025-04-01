using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    Runes runes;

    private void Start()
    {
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
    }

    public void Update()
    {
        if (!runes.inventoryIsOpened)
        {
            Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y * -1, dir.x * -1) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
