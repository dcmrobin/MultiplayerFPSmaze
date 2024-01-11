using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Seed : NetworkBehaviour
{
    public string seed;
    public TMP_InputField worldSizeInputField;
    public int worldSize;
    /*int index;

    public void Update()
    {
        Debug.Log(seed[index].ToString());
        index += 1;
    }*/

    public void SetSeed()
    {
        if (worldSizeInputField.text != "")
        {
            worldSize = Mathf.Abs(System.Int32.Parse(worldSizeInputField.text));
        }
        else
        {
            worldSize = 73;
        }
    }
}
