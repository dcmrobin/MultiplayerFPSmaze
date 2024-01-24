using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    public NetworkVariable<bool> doorOpen = new NetworkVariable<bool>();

    private void Start() {
        if (Random.value > .992)
        {
            ToggleDoorServerRpc(90);
        }
    }

    private void Update() {
        if (UnityEngine.Random.value > .9995)
        {
            if (!doorOpen.Value)
            {
                ToggleDoorServerRpc(90);
            }
            else
            {
                ToggleDoorServerRpc(-90);
            }
        }
    }

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
}
