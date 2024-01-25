using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Lift : NetworkBehaviour
{
    public Transform liftGround;
    public NetworkVariable<bool> hasDescended = new NetworkVariable<bool>();

    [ServerRpc(RequireOwnership = false)]
    public void ActivateLiftServerRpc()
    {
        if (!hasDescended.Value)
        {
            //descend
            hasDescended.Value = true;
            //can't go past -39.5 y value
        }
        else if (hasDescended.Value)
        {
            //ascend
            hasDescended.Value = false;
            //can't go past 0.5 y value
        }
    }
}
