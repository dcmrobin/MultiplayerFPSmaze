using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TempAdoptPlayer : NetworkBehaviour
{
    GameObject adoptedPlayer;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            //other.transform.SetParent(transform);
            adoptedPlayer = other.gameObject;
            SetParentServerRpc(GetComponent<TempAdoptPlayer>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            //other.transform.parent = null;
            RemoveParentServerRpc();
            adoptedPlayer = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetParentServerRpc(NetworkBehaviourReference parent)
    {
        if (parent.TryGet<TempAdoptPlayer>(out TempAdoptPlayer dp))
        {
            adoptedPlayer.GetComponent<NetworkObject>().TrySetParent(dp.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveParentServerRpc()
    {
        adoptedPlayer.GetComponent<NetworkObject>().TryRemoveParent();
    }
}
