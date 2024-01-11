using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [Header("Segment variables")]
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
    bool giveUp;
    int flickerNum;

    private void Start() {
        flickerNum = Random.Range(0, 10);

        if (name != "Start")
        {
            GenerateSegment();
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

        if (giveUp)
        {
            for (int i = 0; i < exits.Length; i++)
            {
                if (exits[i].childCount == 0)
                {
                    Instantiate(wallPrefab, exits[i]);
                    giveUp = false;
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
                        GameObject newSegment = Instantiate(segmentPrefabList[randomNum], exits[j]);
                    }
                    else
                    {
                        goto regen;
                    }
                }
        }

        giveUp = true;
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
