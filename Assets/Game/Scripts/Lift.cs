using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Lift : NetworkBehaviour
{
    public Transform liftGround;
    public NetworkVariable<bool> hasDescended = new NetworkVariable<bool>();
    bool descending;
    bool ascending;

    [ServerRpc(RequireOwnership = false)]
    public void ActivateLiftServerRpc()
    {
        if (!hasDescended.Value)
        {
            ascending = false;
            descending = true;
            hasDescended.Value = true;
            //can't go past -39.5 y value
        }
        else if (hasDescended.Value)
        {
            descending = false;
            ascending = true;
            hasDescended.Value = false;
            //can't go past 0.5 y value
        }
    }

    private void Update() {
        if (descending && liftGround.position.y > -39.5)
        {
            liftGround.Translate(-liftGround.up);
        }
        else if (ascending && liftGround.position.y < 0.5)
        {
            liftGround.Translate(liftGround.up);
        }
    }
}
