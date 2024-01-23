using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthMarker : MonoBehaviour
{
    public Transform target;

    private void Update() {
        if (target != null)
        {
            Vector3 othMarkerTargetPos = transform.parent.parent.parent.parent.GetComponent<PlayerController>().mapCamera.GetComponentInChildren<Camera>().WorldToScreenPoint(target.position);
            float padding = 10f;
            Rect screenBounds = new Rect(padding, padding, Screen.width - 2 * padding, Screen.height - 2 * padding);
        
            othMarkerTargetPos.x = Mathf.Clamp(othMarkerTargetPos.x, screenBounds.x, screenBounds.xMax);
            othMarkerTargetPos.y = Mathf.Clamp(othMarkerTargetPos.y, screenBounds.y, screenBounds.yMax);
            transform.position = new Vector3(othMarkerTargetPos.x, othMarkerTargetPos.y, 0);

            if (target == transform.parent.parent.parent.parent)
            {
                Destroy(gameObject);
            }
        }
    }
}
