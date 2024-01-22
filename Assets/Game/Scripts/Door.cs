using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void ToggleDoorServerRpc(float rotation)
    {
        transform.localRotation = Quaternion.Euler(0, rotation, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetParentServerRpc(NetworkBehaviourReference parent)
    {
        if (parent.TryGet<DoorParent>(out DoorParent dp))
        {
            GetComponent<NetworkObject>().TrySetParent(dp.gameObject);
        }
    }

    //[ClientRpc]
    //public void ToggleDoorClientRpc(float rotation)
    //{
    //    transform.localRotation = Quaternion.Euler(0, rotation, 0);
    //}
}
