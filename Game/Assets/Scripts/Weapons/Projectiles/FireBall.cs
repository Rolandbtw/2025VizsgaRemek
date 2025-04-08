using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("GameObjects and Transforms")]
    [SerializeField] GameObject fireBallEffect;
    [SerializeField] GameObject fireWave;
    [Header("Floats to customize")]
    [SerializeField] float fallSpeed;
    [SerializeField] float locationThreshold = 0.1f;
    [SerializeField] float rotationSpeed;
    public float damage;

    private Vector3 mousePosition;
    private Vector3 direction;
    Sounds soundScript;
    void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = transform.position.z;
        direction = (mousePosition - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * fallSpeed * Time.deltaTime;

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime*-1);

        if (Vector3.Distance(transform.position, mousePosition) < locationThreshold || transform.position.y<mousePosition.y)
        {
            soundScript.MakeSound("meteorSound", 0.5f);
            Instantiate(fireBallEffect, transform.position, fireBallEffect.transform.rotation);
            GameObject wave=Instantiate(fireWave, transform.position, fireBallEffect.transform.rotation);
            ImpactWave impactScript = wave.GetComponent<ImpactWave>();
            if (impactScript != null)
            {
                impactScript.damage = damage;
            }
            Destroy(gameObject);
        }
    }
}
