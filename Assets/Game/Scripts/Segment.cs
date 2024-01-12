using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Segment : NetworkBehaviour
{
    [Header("Segment variables")]
    public bool isStart;
    public int rarity;
    public GameObject wallPrefab;
    public GameObject[] segmentPrefabList;
    public Transform[] exits;

    [Header("Light-flicker variables")]
    public float maxWait = 2;
    public float maxFlicker = 0.2f;
    float timer;
    float interval;

    [HideInInspector] public bool isOverlapping;
    bool startedGenerating;
    bool finishedGenerating;
    int flickerNum;

    private void Awake() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Start() {
        flickerNum = Random.Range(0, 10);

        if (!isStart)
        {
            GenerateSegment();
        }
    }

    void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Client " + clientId + " connected!");
            PlayerController connectedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerController>();
            connectedClient.recievedString.Value = GameObject.Find("Start").GetComponent<Seed>().seed;
            //SEND SEED
        }
    }

    private void Update() {
        if (isOverlapping)
        {
            Destroy(gameObject);
        }

        if (transform.Find("Light") != null)
        {
            if (Camera.main != null)
            {
                if (Vector3.Distance(transform.position, Camera.main.transform.position) >= 300)
                {
                    transform.Find("Light").gameObject.SetActive(false);
                }
                else if (Vector3.Distance(transform.position, Camera.main.transform.position) < 300)
                {
                    transform.Find("Light").gameObject.SetActive(true);
                }
            }
            else//this is gonna get mixed up between the players
            {
                transform.Find("Light").gameObject.SetActive(false);
            }

            if (!transform.Find("Light").Find("Bulb").GetComponent<Light>().enabled)
            {
                transform.Find("Light").Find("Bulb").gameObject.SetActive(false);
            }
            else if (transform.Find("Light").Find("Bulb").GetComponent<Light>().enabled)
            {
                transform.Find("Light").Find("Bulb").gameObject.SetActive(true);
            }
        }

        if (startedGenerating && GameObject.Find("Start").GetComponent<Seed>().worldSize < 1)
        {
            finishedGenerating = true;
        }

        if (finishedGenerating)
        {
            for (int i = 0; i < exits.Length; i++)
            {
                if (exits[i].childCount == 0)
                {
                    Instantiate(wallPrefab, exits[i]);
                    finishedGenerating = false;
                }
            }
        }

        if (transform.Find("Light") != null && flickerNum > 8)
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                ToggleLight();
            }
        }
    }

    public void GenerateSegment() {
        startedGenerating = true;
        if (GameObject.Find("Start").GetComponent<Seed>().worldSize > -1)
        {
            for (int j = 0; j < exits.Length; j++)
            {
                regen:
                    if (exits[j].childCount == 0)
                    {
                        int randomNum = Random.Range(0, segmentPrefabList.Length);
                        int randProb1 = Random.Range(0, segmentPrefabList[randomNum].GetComponent<Segment>().rarity);
                        int randProb2 = Random.Range(0, segmentPrefabList[randomNum].GetComponent<Segment>().rarity);
                        if (randProb1 == randProb2)
                        {
                            GameObject.Find("Start").GetComponent<Seed>().seed += randomNum.ToString();
                            GameObject.Find("Start").GetComponent<Seed>().worldSize -= 1;
                            GameObject newSegment = Instantiate(segmentPrefabList[randomNum], exits[j]);
                        }
                        else
                        {
                            goto regen;
                        }
                    }
            }
        }
    }

    void ToggleLight()
    {
        transform.Find("Light").Find("Bulb").GetComponent<Light>().enabled = !transform.Find("Light").Find("Bulb").GetComponent<Light>().enabled;
        if (transform.Find("Light").Find("Bulb").GetComponent<Light>().enabled)
        {
            interval = Random.Range(0, maxWait);
        }
        else 
        {
            interval = Random.Range(0, maxFlicker);
        }
        
        timer = 0;
    }
}
