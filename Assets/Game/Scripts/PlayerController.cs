using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;
using Unity.Services.Authentication;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<FixedString4096Bytes> recievedString = new NetworkVariable<FixedString4096Bytes>();
    public NetworkVariable<FixedString4096Bytes> recievedVentString = new NetworkVariable<FixedString4096Bytes>();
    [Header("Player Variables")]
    [Tooltip("Pause menu object")]
    public GameObject pauseMenu;

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

    [Tooltip("The player's crosshair image")]
    public Image crosshair;

    [Tooltip("Gun impact fx")]
    public GameObject impactParticleSystemPrefab;

    [Tooltip("Everything except the overlapcolliders")]
    public LayerMask everythingMask;

    private float verticalRotation = 0f;
    private Rigidbody rb;
    private bool isLookingAtMap;
    public bool isReading;
    private GameObject[] playerCameras;
    public GameObject doorPrefab;

    RaycastHit hit;
    RaycastHit bulletHit;
    public int nextScene;

    private void Awake() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        //
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
            GameObject.Find("Start").GetComponent<Segment>().LateGenerateSegment(recievedString.Value.ToString(), recievedVentString.Value.ToString());
        }
    }

    public void Exit(bool disconnecting)
    {
        if (disconnecting)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("dungeon", LoadSceneMode.Single);
            AuthenticationService.Instance.SignOut();
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            Application.Quit();
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (transform.position.y < -500)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(nextScene);
            Destroy(GameObject.Find("NetworkManager"));
            Destroy(gameObject);
        }

        if (!isReading)
        {
            if (!isLookingAtMap && !pauseMenu.activeSelf)
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
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

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
                }
            }
            GameObject.Find("Start").GetComponent<Segment>().backupTime = false;
        }

        if (GameObject.Find("ingameTextCanvas") != null && GameObject.Find("ingameTextCanvas").activeSelf)
        {
            isReading = true;
            GameObject.Find("ingameTextCanvas").transform.Find("TextPanel").Find("Text").GetComponent<TMP_Text>().text = "Welcome! Thank you for signing up for the testing of our all-new harmless firearms! They may look mean but they don't actually hurt anything.\nAnyway, here's a little intro: this place is actually a refurbished section of the [REDACTED], and those dead ends you'll encounter were put there in order to separate you from the rest of the (as far as we can tell, infinite) space of the [REDACTED]. Ignore the random openings and closings of the doors, that's justs something we couldn't fix. We apologize for the flickering lights, we couldn't figure out what was doing that, since during the refurbishment we replaced those horrible fluorescent lights. Seems like this place has an infinite source of energy as well as space.\nJust in case you somehow get lost, we've provided you with a handheld map device, and we've also got a safeguard for when you inevitably get the the lift to go down when you are underneath it. People these days.\n\n\nAnyway, have a great time!\n\n                          - CETC Co.";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (!isLookingAtMap && !isReading && !pauseMenu.activeSelf)
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
                mapCamera.GetComponentInChildren<MapCam>().ScanForPlayers();
                transform.Find("Canvas").gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                isLookingAtMap = false;
                mapCamera.GetComponentInChildren<MapCam>().DiscardOthPlayerMarkers();
                mapCamera.SetActive(false);
                transform.Find("Canvas").gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, interactionMask))
        {
            crosshair.color = Color.yellow;
            GameObject currentObject = hit.collider.gameObject;
            if (Input.GetMouseButtonDown(0))
            {
                if (currentObject.CompareTag("Door"))
                {
                    if (!currentObject.transform.parent.GetComponent<Door>().doorOpen.Value)
                    {
                        //currentObject.transform.parent.localRotation = Quaternion.Euler(0, 90, 0);
                        currentObject.transform.parent.GetComponent<Door>().ToggleDoorServerRpc(90);
                        //currentObject.transform.parent.GetComponent<Door>().ToggleDoorClientRpc(90);
                    }
                    else if (currentObject.transform.parent.GetComponent<Door>().doorOpen.Value)
                    {
                        //currentObject.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
                        currentObject.transform.parent.GetComponent<Door>().ToggleDoorServerRpc(-90);
                        //currentObject.transform.parent.GetComponent<Door>().ToggleDoorClientRpc(0);
                    }
                }
                else if (currentObject.CompareTag("Vent"))
                {
                    transform.position = new Vector3(currentObject.GetComponent<Vent>().closestVent.parent.position.x, currentObject.GetComponent<Vent>().closestVent.parent.position.y, currentObject.GetComponent<Vent>().closestVent.parent.position.z);
                }
                else if (currentObject.CompareTag("Button"))
                {
                    currentObject.GetComponent<InteractButton>().Interact();
                }
            }
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

    public void LockCursor(){Cursor.lockState = CursorLockMode.Locked;}

    public void HandleGun()
    {
        if (Input.GetMouseButton(1))
        {
            mainCamera.transform.Find("Gun").Find("Muzzle").Find("Flash").GetComponent<ParticleSystem>().Play();
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, everythingMask))
            {
                GameObject impactFX = Instantiate(impactParticleSystemPrefab, hit.point, transform.rotation);
                impactFX.GetComponent<ParticleSystem>().Play();
                Destroy(impactFX, .5f);

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

        if (currentHealth <= 0)
        {
            transform.position = new Vector3(0, 0, 0);
            currentHealth = maxHealth;
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
