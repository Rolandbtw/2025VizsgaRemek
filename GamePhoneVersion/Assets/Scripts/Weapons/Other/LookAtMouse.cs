using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.GridBrushBase;

public class LookAtMouse : MonoBehaviour
{
    Runes runes;
    public Vector3 dir;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        if (movementActions == null)
        {
            movementActions = new PlayerMovementInputActions();
        }
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        PlayerPrefs.SetInt("AimAssist", 1);
        runes = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
    }

    public void Update()
    {
        if (PlayerPrefs.GetInt("AimAssist") == 0)
        {
            LookAtDirection();
        }
        else
        {
            GameObject closestObject = GetClosestVisibleObjectWithTag("Enemy");
            if (closestObject != null)
            {
                LookAtEnemy(closestObject);
            }
            else
            {
                LookAtDirection();
            }
        }
    }

    public GameObject GetClosestVisibleObjectWithTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        Camera mainCamera = Camera.main;

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject obj in objectsWithTag)
        {
            if (IsVisibleToCamera(obj, mainCamera) && obj.GetComponent<EnemyHealth>().healhPoints>0)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }
        return closestObject;
    }

    bool IsVisibleToCamera(GameObject obj, Camera camera)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.isVisible)
        {
            return true;
        }

        Vector3 screenPoint = camera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    void LookAtEnemy(GameObject target)
    {
        if (!runes.inventoryIsOpened)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.Normalize();

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180;

            Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000 * Time.deltaTime);
        }

        BasicSlicing basicSlicing=GetComponent<BasicSlicing>();
        if(basicSlicing != null)
        {
            basicSlicing.isUsingAimAssist = true;

            if (target.transform.position.x > transform.position.x)
            {
                basicSlicing.rotationDirection = 1;
            }
            else
            {
                basicSlicing.rotationDirection = -1;
            }
        }
    }

    void LookAtDirection()
    {
        BasicSlicing basicSlicing = GetComponent<BasicSlicing>();
        if (basicSlicing != null)
        {
            basicSlicing.isUsingAimAssist = false;
        }

        Vector2 joystickVector = movementActions.PlayerMap.Movement.ReadValue<Vector2>();

        if (!runes.inventoryIsOpened && joystickVector != new Vector2(0, 0))
        {
            dir = new Vector3(joystickVector.x, joystickVector.y, 0);
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y * -1, dir.x * -1) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
