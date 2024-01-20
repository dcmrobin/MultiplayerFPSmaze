using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Collections;
using TMPro;
using Unity.Netcode.Components;
using System.Globalization;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<FixedString4096Bytes> recievedString = new NetworkVariable<FixedString4096Bytes>();
    [Header("Player Variables")]
    [Tooltip("How sensitive is the mouse look?")]
    public float mouseSensitivity = 2.0f;

    [Tooltip("How fast can the player move?")]
    private float moveSpeed = 17;

    [Tooltip("How high can the player jump?")]
    public float jumpForce = 8.0f;

    [Tooltip("The max health of the player. (this won't change in-game)")]
    public int maxHealth = 100;

    [Tooltip("The current health of the player. (this will change in-game)")]
    public int currentHealth;

    [Tooltip("The player's UI health number")]
    public TMP_Text UIhealthNum;

    [Tooltip("The damage of the player's gun")]
    public int gunDamage = 1;

    [Tooltip("The player's flashlight")]
    public GameObject flashlight;

    [Tooltip("The map camera")]
    public GameObject mapCamera;

    [Tooltip("The main camera")]
    public GameObject mainCamera;

    [Tooltip("Layermask for shooting")]
    public LayerMask playermask;

    [Header("Interaction variables")]
    [Tooltip("What layers can the player interact with?")]
    public LayerMask interactionMask;
    [Tooltip("How far can the player be away from the thing they're trying to interact with?")]
    public float maxDistance = 10.0f;

    private float verticalRotation = 0f;
    private Rigidbody rb;
    private bool isLookingAtMap;
    private GameObject[] playerCameras;
    public GameObject doorPrefab;

    RaycastHit hit;
    RaycastHit bulletHit;

    private void Awake() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        //if (IsServer)
        //{
            //PlayerController connectedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerController>();
            //if (connectedClient.gameObject != gameObject)
            //{
            //    connectedClient.transform.Find("Canvas").gameObject.SetActive(false);
            //}
        //}
    }

    void Start()
    {
        currentHealth = maxHealth;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (GameObject.Find("Start").GetComponent<Segment>().exits[0].childCount == 0)
        {
            GameObject.Find("Start").GetComponent<Segment>().keepGenerating = true;
            GameObject.Find("Start").GetComponent<Segment>().LateGenerateSegment(recievedString.Value.ToString());
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (!isLookingAtMap)
        {
            HandleMouseLook();
            HandleJump();
            HandleGun();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = 27;
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = 17;
            }

            UIhealthNum.text = currentHealth.ToString();
        }
        HandleInteractions();

        playerCameras = GameObject.FindGameObjectsWithTag("MainCamera");
        for (int i = 0; i < playerCameras.Length; i++)
        {
            if (playerCameras[i] != mainCamera)
            {
                playerCameras[i].SetActive(false);
                playerCameras[i].transform.parent.Find("Canvas").gameObject.SetActive(false);
            }
        }

        if (IsServer && GameObject.Find("Start").GetComponent<Segment>().backupTime)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("DoorRoot").Length; i++)
            {
                if (GameObject.FindGameObjectsWithTag("DoorRoot")[i].transform.childCount == 0)
                {
                    GameObject door = Instantiate(doorPrefab, GameObject.FindGameObjectsWithTag("DoorRoot")[i].transform);
                    door.GetComponent<NetworkObject>().Spawn();
                    //door.transform.SetParent(GameObject.FindGameObjectsWithTag("DoorRoot")[i].transform);
                    door.GetComponent<Door>().SetParentServerRpc(GameObject.FindGameObjectsWithTag("DoorRoot")[i].GetComponent<DoorParent>());
                }
            }
            GameObject.Find("Start").GetComponent<Segment>().backupTime = false;
        }
        else if (!IsServer && GameObject.Find("Start").GetComponent<Segment>().backupTime)
        {
            for (int j = 0; j < GameObject.FindGameObjectsWithTag("Door").Length; j++)
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("DoorRoot").Length; i++)
                {
                    if (GameObject.FindGameObjectsWithTag("DoorRoot")[i].transform.childCount == 0)
                    {
                        GameObject.FindGameObjectsWithTag("Door")[j].GetComponent<Door>().SetParentServerRpc(GameObject.FindGameObjectsWithTag("DoorRoot")[i].GetComponent<DoorParent>());
                    }
                }
            }
            GameObject.Find("Start").GetComponent<Segment>().backupTime = false;
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
                if (currentObject.name == "Door")
                {
                    if (currentObject.transform.parent.localRotation != Quaternion.Euler(0, 90, 0))
                    {
                        //currentObject.transform.parent.localRotation = Quaternion.Euler(0, 90, 0);
                        currentObject.transform.parent.GetComponent<Door>().ToggleDoorServerRpc(90);
                        //currentObject.transform.parent.GetComponent<Door>().ToggleDoorClientRpc(90);
                    }
                    else
                    {
                        //currentObject.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
                        currentObject.transform.parent.GetComponent<Door>().ToggleDoorServerRpc(0);
                        //currentObject.transform.parent.GetComponent<Door>().ToggleDoorClientRpc(0);
                    }
                }
                else if (currentObject.name == "Vent")
                {
                    transform.position = new Vector3(currentObject.GetComponent<Vent>().closestVent.parent.position.x, currentObject.GetComponent<Vent>().closestVent.parent.position.y, currentObject.GetComponent<Vent>().closestVent.parent.position.z);
                }
            }
        }
    }

    public void HandleGun()
    {
        if (Input.GetMouseButton(1))
        {
            mainCamera.transform.Find("Gun").Find("Muzzle").Find("Flash").GetComponent<ParticleSystem>().Play();
            //YEAH AND MAKE A LITTLE SPARK THING APPEAR AT HIT.POINT
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, playermask))
            {
                if (IsClient)
                {
                    hit.collider.GetComponent<PlayerController>().TakeDamageServerRpc(gunDamage);
                }
                hit.collider.GetComponent<PlayerController>().TakeDamageClientRpc(gunDamage);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int amount)
    {
        currentHealth -= amount;
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int amount)
    {
        currentHealth -= amount;
    }
}
