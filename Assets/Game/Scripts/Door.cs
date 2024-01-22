using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    public NetworkVariable<bool> doorOpen = new NetworkVariable<bool>();

    [ServerRpc(RequireOwnership = false)]
    public void ToggleDoorServerRpc(float rotation)
    {
        if (!doorOpen.Value)
        {
            doorOpen.Value = true;
        }
        else if (doorOpen.Value)
        {
            doorOpen.Value = false;
        }

        transform.Rotate(0, rotation, 0);
    }

    //[ClientRpc]
    //public void ToggleDoorClientRpc(float rotation)
    //{
    //    transform.localRotation = Quaternion.Euler(0, rotation, 0);
    //}
}
