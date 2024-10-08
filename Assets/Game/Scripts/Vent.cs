using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

public class Vent : NetworkBehaviour
{
    public Transform closestVent;
    public bool hasWorldGenerated;
    GameObject[] allVents;
    List<float> allDistances = new List<float>();

    private void Update() {
        if (hasWorldGenerated)
        {
            if (closestVent == null)
            {
                allVents = GameObject.FindGameObjectsWithTag("Vent");
                for (int i = 0; i < allVents.Length; i++)
                {
                    if (allVents[i] != gameObject)
                    {
                        allDistances.Add(Vector3.Distance(allVents[i].transform.position, transform.position));
                    }
                }
                for (int j = 0; j < allVents.Length; j++)
                {
                    if (Vector3.Distance(allVents[j].transform.position, transform.position) == allDistances.Min())
                    {
                        closestVent = allVents[j].transform;
                        allVents[j].GetComponent<Vent>().closestVent = gameObject.transform;
                    }
                }
                if (closestVent == null)
                {
                    Destroy(gameObject);
                }
            }
            //else
            //{
            //    enabled = false;
            //}
        }

        if (Random.value > .99999 && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > 70)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
