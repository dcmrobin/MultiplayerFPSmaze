using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractButton : NetworkBehaviour
{
    public enum Action{ActivateLift}
    public Action onPress;
    public void Interact()
    {
       if (onPress == Action.ActivateLift)
       {
            transform.parent.GetComponent<Lift>().ActivateLiftServerRpc();
       }
    }
}
