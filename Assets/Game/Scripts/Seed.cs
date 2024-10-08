using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Seed : NetworkBehaviour
{
    public string seed;
    public string ventSeed;
    public TMP_InputField worldSizeInputField;
    public int worldSize;
    public int counter = 0;

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
