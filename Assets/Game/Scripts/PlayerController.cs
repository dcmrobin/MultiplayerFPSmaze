using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public string recievedString;
    [Header("Player Variables")]
    [Tooltip("How sensitive is the mouse look?")]
    public float mouseSensitivity = 2.0f;
    [Tooltip("How fast can the player move?")]
    public float moveSpeed = 5.0f;
    [Tooltip("How high can the player jump?")]
    public float jumpForce = 8.0f;
    [Tooltip("The player's flashlight")]
    public GameObject flashlight;
    [Tooltip("The map camera")]
    public GameObject mapCamera;
    [Tooltip("The main camera")]
    public GameObject mainCamera;

    [Header("Interaction variables")]
    [Tooltip("What layers can the player interact with?")]
    public LayerMask interactionMask;
    [Tooltip("How far can the player be away from the thing they're trying to interact with?")]
    public float maxDistance = 10.0f;

    private float verticalRotation = 0f;
    private Rigidbody rb;
    private bool isLookingAtMap;
    private GameObject[] playerCameras;

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
        if (!IsOwner) return;

        if (!isLookingAtMap)
        {
            HandleMouseLook();
            HandleJump();
        }
        HandleInteractions();

        playerCameras = GameObject.FindGameObjectsWithTag("MainCamera");
        for (int i = 0; i < playerCameras.Length; i++)
        {
            if (playerCameras[i] != mainCamera)
            {
                playerCameras[i].SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (!isLookingAtMap)
        {
            HandlePlayerMovement();
        }
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
        return Physics.Raycast(transform.position, Vector3.down, 6f);
    }

    void HandleInteractions()
    {
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            if (!flashlight.activeSelf)
            {
                flashlight.SetActive(true);
            }
            else
            {
                flashlight.SetActive(false);
            }
        }*/

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isLookingAtMap)
            {
                isLookingAtMap = true;
                mapCamera.SetActive(true);
                transform.Find("Canvas").gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                isLookingAtMap = false;
                mapCamera.SetActive(false);
                transform.Find("Canvas").gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

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
