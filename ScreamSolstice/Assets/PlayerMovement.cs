using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 12.0f;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float jumpHeight = 3.0f;

    private CharacterController controller;

    private Transform cam;

    private float velocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = transform.GetChild(0);
    }

    private void LateUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            }
            else
            {
                velocity = -2.0f;
            }
        }

        velocity += gravity * Time.deltaTime;

        Vector3 move = Vector3.Normalize(transform.right * x + transform.forward * z) * speed;

        move.y = velocity;

        controller.Move(move * Time.deltaTime);
    }
}
