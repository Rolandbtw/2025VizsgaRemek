using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFlameFloat : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float moveDistance = 0.005f;
    private Vector3 startPosition;
    private Vector3 childStartPosition;
    private float t = 0;

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Lerp(startPosition.y - moveDistance, startPosition.y + moveDistance, Mathf.PingPong(t, 1));
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        t += Time.deltaTime * moveSpeed;
    }
}
