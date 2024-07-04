using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeLight : MonoBehaviour
{
    Transform player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, player.position) > 250)
        {
            GetComponentInChildren<Light>().enabled = false;
        }
        else
        {
            GetComponentInChildren<Light>().enabled = true;
        }

        /*if (Vector3.Distance(transform.position, player.position) > 30)
        {
            GetComponentInChildren<AudioSource>().enabled = false;
        }
        else
        {
            GetComponentInChildren<AudioSource>().enabled = true;
        }*/
    }
}
