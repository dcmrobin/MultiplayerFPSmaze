using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BackupExit : NetworkBehaviour
{
    public GameObject wallPrefab;

    private void Update() {
        if (GetComponentInParent<Segment>().backupTime)
        {
            if (transform.childCount == 0)
            {
                GameObject wall = Instantiate(wallPrefab, transform);
                wall.name = "backupWall";
                enabled = false;
            }
        }
    }

    public void GenerateWall()
    {
        if (transform.childCount == 0)
        {
            GameObject wall = Instantiate(wallPrefab, transform);
            wall.name = "backupWall";
            enabled = false;
        }
    }
}
