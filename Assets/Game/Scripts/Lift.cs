using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Lift : NetworkBehaviour
{
    public bool isLiftUp;
    public Transform liftGround;
    public bool descending;
    public bool ascending;
    public GameObject liftFloorPrefab;
    public GameObject parentPrefab;

    private void Update() {
        if (liftGround == null)
        {
            if (IsServer)
            {
                GetGroundServerRpc();
            }
            GetGround();
        }
        else
        {
            if (descending && liftGround.localPosition.y > -39.5)
            {
                liftGround.localPosition = Vector3.MoveTowards(liftGround.localPosition, new Vector3(0, -39.5f, 30), 1);
            }
            else if (ascending && liftGround.localPosition.y < 0.5)
            {
                liftGround.localPosition = Vector3.MoveTowards(liftGround.localPosition, new Vector3(0, 0.5f, 30), 1);
            }

            if (liftGround.GetComponent<TempAdoptPlayer>().hasDescended.Value)
            {
                ascending = false;
                descending = true;
            }
            else if (!liftGround.GetComponent<TempAdoptPlayer>().hasDescended.Value)
            {
                descending = false;
                ascending = true;
            }
        }
    }

    public void GetGround()
    {
        Collider[] colliders = Physics.OverlapBox(transform.Find("OverlapBox").position, transform.Find("OverlapBox").localScale, Quaternion.identity);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("LiftFloor"))
            {
                liftGround = col.transform;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetGroundServerRpc()
    {
        GameObject liftFloor = null;
        liftFloor = Instantiate(liftFloorPrefab, transform);
        GameObject parent = Instantiate(parentPrefab, transform);
        parent.GetComponent<NetworkObject>().Spawn();
        liftFloor.name = "LiftFloor";
        liftFloor.GetComponent<NetworkObject>().Spawn();
        liftFloor.GetComponent<NetworkObject>().TrySetParent(parent.transform);
        liftGround = liftFloor.transform;

        if (isLiftUp)
        {
            liftGround.GetComponent<TempAdoptPlayer>().SetPos();
        }
    }
}
