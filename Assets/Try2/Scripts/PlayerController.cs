using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player controls")]
    [Tooltip("How sensitive is the mouse look?")]
    public float mouseSensitivity = 2.0f;
    [Tooltip("How fast can the player move?")]
    public float moveSpeed = 5.0f;
    [Tooltip("How high can the player jump?")]
    public float jumpForce = 8.0f;

    [Header("Interaction variables")]
    [Tooltip("What layers can the player interact with?")]
    public LayerMask interactionMask;
    [Tooltip("How far can the player be away from the thing they're trying to interact with?")]
    public float maxDistance = 10.0f;

    private float verticalRotation = 0f;
    private Rigidbody rb;

    RaycastHit hit;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
        HandleInteractions();
    }

    void FixedUpdate()
    {
        HandlePlayerMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandlePlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveVelocity = transform.TransformDirection(moveDirection) * moveSpeed;

        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 5.01f);
    }

    void HandleInteractions()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, interactionMask))
        {
            GameObject currentObject = hit.collider.gameObject;
            if (Input.GetMouseButtonDown(0))
            {
                if (currentObject.name == "Door" && currentObject.transform.parent.localRotation != Quaternion.Euler(0, 90, 0))
                {
                    currentObject.transform.parent.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    currentObject.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }
}
