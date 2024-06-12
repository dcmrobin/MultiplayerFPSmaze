using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractButton : NetworkBehaviour
{
    public enum Action{ActivateLift, ShowSomething}
    public Action onPress;
    public GameObject thingToShow;
    public void Interact()
    {
          GetComponent<AudioSource>().Play();
          if (onPress == Action.ActivateLift)
          {
               transform.parent.GetComponent<AudioSource>().Play();
               transform.parent.GetComponent<Lift>().liftGround.GetComponent<TempAdoptPlayer>().ActivateLiftServerRpc();
          }
          else if (onPress == Action.ShowSomething)
          {
               thingToShow.SetActive(true);
          }
    }
}
