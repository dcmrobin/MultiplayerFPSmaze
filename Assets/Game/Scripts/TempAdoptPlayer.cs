using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TempAdoptPlayer : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }
}
