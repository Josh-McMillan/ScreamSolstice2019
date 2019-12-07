using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 50.0f;

    [SerializeField] private float snappiness = 20.0f;

    [SerializeField] private float clampAngle = 75.0f;

    private Transform playerBody;

    private Vector2 lookRotation = Vector2.zero;

    private float xAccumulator;
    private float yAccumulator;

    private void Start()
    {
        playerBody = transform.parent;
    }

    private void LateUpdate()
    {
        lookRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        lookRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xAccumulator = Mathf.Lerp(xAccumulator, lookRotation.x, snappiness * Time.deltaTime);
        yAccumulator = Mathf.Lerp(yAccumulator, lookRotation.y, snappiness * Time.deltaTime);

        yAccumulator = Mathf.Clamp(yAccumulator, -clampAngle, clampAngle);

        playerBody.localEulerAngles = new Vector3(0.0f, xAccumulator, 0.0f);
        transform.localEulerAngles = new Vector3(yAccumulator, 0.0f, 0.0f);
    }
}
