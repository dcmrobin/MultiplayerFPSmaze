using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public GameObject[] segmentPrefabList;
    public Transform[] exits;

    private void Start() {
        GenerateSegment();
    }

    void GenerateSegment() {
        int randomNum = Random.Range(0, segmentPrefabList.Length);

        for (int j = 0; j < exits.Length; j++)
        {
            if (exits[j].childCount == 0)
            {
                GameObject newSegment = Instantiate(segmentPrefabList[randomNum], exits[j]);
            }
        }
    }
}
