using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public int rarity;
    public GameObject wallPrefab;
    public GameObject[] segmentPrefabList;
    public Transform[] exits;
    [HideInInspector] public bool isOverlapping;
    bool giveUp;

    private void Start() {
        /*if (Random.value > 0.6)
        {
            if (transform.Find("Light") != null)
            {
                transform.Find("Light").gameObject.SetActive(false);
            }
        }*/

        GenerateSegment();
    }

    private void Update() {
        if (isOverlapping)
        {
            Destroy(gameObject);
        }

        if (transform.Find("Light") != null)
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
    }

    void GenerateSegment() {
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
}
