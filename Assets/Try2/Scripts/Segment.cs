using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public GameObject[] segmentPrefabList;
    public Transform[] exits;

    private void Start() {
        int randomSegmentNum = Random.Range(0, segmentPrefabList.Length);

        for (int j = 0; j < exits.Length; j++)
        {
            if (exits[j].childCount == 0)
            {
                GameObject newSegment = Instantiate(segmentPrefabList[randomSegmentNum], exits[j]);
            }
        }
    }
}
