using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWithinRange : MonoBehaviour
{
    [Tooltip("If false, it will deactivate instead of activate this object")]
    public bool activate;
    public float range;
    public Transform objectToCompareRangeTo;
    public GameObject objectToToggle;

    private void Update() {
        if (Vector3.Distance(transform.position, objectToCompareRangeTo.position) <= range)
        {
            objectToToggle.SetActive(activate);
        }
    }
}
