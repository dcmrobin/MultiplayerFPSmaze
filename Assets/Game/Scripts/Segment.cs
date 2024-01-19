using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.NetStatsMonitor;
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

    [HideInInspector] public bool isOverlapping;
    [HideInInspector] public bool keepGenerating;
    bool startedGenerating;
    bool finishedGenerating;
    [HideInInspector] public bool backupTime;
    //public int segmentSeedNum;

    private void Awake() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Start() {
        if (!isStart)
        {
            if (!GameObject.Find("Start").GetComponent<Segment>().keepGenerating)
            {
                GenerateSegment();
            }
            if (GameObject.Find("Start").GetComponent<Segment>().keepGenerating)
            {
                LateGenerateSegment(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().recievedString.Value.ToString());
            }
        }
    }

    void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Client " + clientId + " connected!");
            PlayerController connectedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerController>();
            connectedClient.recievedString.Value = GameObject.Find("Start").GetComponent<Seed>().seed;
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
            startedGenerating = false;
            finishedGenerating = true;
        }

        if (finishedGenerating)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Vent").Length; i++)
            {
                GameObject.FindGameObjectsWithTag("Vent")[i].GetComponent<Vent>().hasWorldGenerated = true;
            }
            for (int i = 0; i < exits.Length; i++)
            {
                if (exits[i].childCount == 0)
                {
                    GameObject wall = Instantiate(wallPrefab, exits[i]);
                    wall.name = "sadWall";
                }
            }
            finishedGenerating = false;
            backupTime = true;
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
                        int randomNum = UnityEngine.Random.Range(0, segmentPrefabList.Length);
                        int randProb1 = UnityEngine.Random.Range(0, segmentPrefabList[randomNum].GetComponent<Segment>().rarity);
                        int randProb2 = UnityEngine.Random.Range(0, segmentPrefabList[randomNum].GetComponent<Segment>().rarity);
                        if (randProb1 == randProb2)
                        {
                            GameObject.Find("Start").GetComponent<Seed>().seed += randomNum.ToString();
                            GameObject.Find("Start").GetComponent<Seed>().worldSize -= 1;
                            GameObject newSegment = Instantiate(segmentPrefabList[randomNum], exits[j]);
                            if (Convert.ToBoolean(randomNum) && transform.Find("VentTarget") != null)
                            {
                                Destroy(transform.Find("VentTarget").gameObject);
                            }
                            //newSegment.GetComponent<Segment>().segmentSeedNum = randomNum;
                        }
                        else
                        {
                            goto regen;
                        }
                    }
            }
        }
    }

    public void LateGenerateSegment(string seed)
    {
        for (int j = 0; j < exits.Length; j++)
        {
            if (exits[j].childCount == 0)
            {
                try
                {
                    GameObject newSegment = Instantiate(segmentPrefabList[int.Parse(seed[GameObject.Find("Start").GetComponent<Seed>().counter].ToString())], exits[j]);
                    if (Convert.ToBoolean(int.Parse(seed[GameObject.Find("Start").GetComponent<Seed>().counter].ToString())) && transform.Find("VentTarget") != null)
                    {
                        Destroy(transform.Find("VentTarget").gameObject);
                    }
                    //newSegment.GetComponent<Segment>().segmentSeedNum = int.Parse(seed[GameObject.Find("Start").GetComponent<Seed>().counter].ToString());
                    GameObject.Find("Start").GetComponent<Seed>().counter += 1;
                }
                catch (IndexOutOfRangeException e)
                {
                    GameObject test = Instantiate(wallPrefab, exits[j]);// WHY ISN'T THIS GENERATING
                    test.name = "AAAAAAAAAAAAAAAAAAA";
                    Debug.Log("Guess what? " + e + " Yep, that's right.");
                }

                if (GameObject.Find("Start").GetComponent<Seed>().counter >= seed.Length)
                {
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Vent").Length; i++)
                    {
                        GameObject.FindGameObjectsWithTag("Vent")[i].GetComponent<Vent>().hasWorldGenerated = true;
                    }
                }
            }
        }
    }
}
