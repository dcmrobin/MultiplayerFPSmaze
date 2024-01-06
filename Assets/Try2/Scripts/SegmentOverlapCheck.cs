using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentOverlapCheck : MonoBehaviour
{
    public LayerMask overlapLayermask;

    private void Start() {
        Collider[] overlappingColliders = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().bounds.extents, Quaternion.identity, overlapLayermask);
        if (overlappingColliders.Length > 1)
        {
            transform.parent.GetComponent<Segment>().isOverlapping = true;
        }
    }
}
