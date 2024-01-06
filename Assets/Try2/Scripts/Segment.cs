using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject[] segmentPrefabList;
    public Transform[] exits;
    [HideInInspector] public bool isOverlapping;

    private void Start() {
        if (Random.value > 0.7)
        {
            if (transform.Find("Light") != null)
            {
                transform.Find("Light").gameObject.SetActive(false);
            }
        }

        GenerateSegment();
    }

    private void Update() {
        if (isOverlapping)
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < exits.Length; i++)//This bit might be slowing down the game...
        {
            if (exits[i].childCount == 0)
            {
                Instantiate(wallPrefab, exits[i]);
                enabled = false;
            }
        }
    }

    void GenerateSegment() {
        for (int j = 0; j < exits.Length; j++)
        {
            if (exits[j].childCount == 0)
            {
                int randomNum = Random.Range(0, segmentPrefabList.Length);
                GameObject newSegment = Instantiate(segmentPrefabList[randomNum], exits[j]);
            }
        }
    }
}
