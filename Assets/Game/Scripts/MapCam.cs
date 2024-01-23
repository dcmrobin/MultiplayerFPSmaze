using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MapCam : MonoBehaviour
{
    public GameObject playerMarker;
    public GameObject startMarker;
    public GameObject othMarkerPrefab;
    public GameObject myMarker;

    private void Update() {
        float h = Input.GetAxis("Horizontal");

        //Cursor.lockState = CursorLockMode.None;

        if (GetComponent<Camera>().orthographicSize > 0)
        {
            GetComponent<Camera>().orthographicSize -= Mouse.current.scroll.ReadValue().normalized.y * 50;
        }
        else
        {
            GetComponent<Camera>().orthographicSize = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.parent.Rotate(0, 5, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.parent.Rotate(0, -5, 0);
        }

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.parent.rotation.x == 0)
            {
                transform.parent.rotation = Quaternion.Euler(180, 0, 0);
            }
            else
            {
                transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        transform.parent.position = Camera.main.transform.parent.position;

        Vector3 targetScreenPosition = GetComponent<Camera>().WorldToScreenPoint(Camera.main.transform.parent.position);
        float padding = 10f;
        Rect screenBounds = new Rect(padding, padding, Screen.width - 2 * padding, Screen.height - 2 * padding);

        targetScreenPosition.x = Mathf.Clamp(targetScreenPosition.x, screenBounds.x, screenBounds.xMax);
        targetScreenPosition.y = Mathf.Clamp(targetScreenPosition.y, screenBounds.y, screenBounds.yMax);
        playerMarker.transform.position = new Vector3(targetScreenPosition.x, targetScreenPosition.y, 0);

        Vector3 targetStartScreenPosition = GetComponent<Camera>().WorldToScreenPoint(GameObject.Find("Start").transform.position);

        targetStartScreenPosition.x = Mathf.Clamp(targetStartScreenPosition.x, screenBounds.x, screenBounds.xMax);
        targetStartScreenPosition.y = Mathf.Clamp(targetStartScreenPosition.y, screenBounds.y, screenBounds.yMax);
        startMarker.transform.position = new Vector3(targetStartScreenPosition.x, targetStartScreenPosition.y, 0);

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().mapCamera.GetComponentInChildren<MapCam>().myMarker != null)
            {
                Vector3 othMarkerTargetPos = GetComponent<Camera>().WorldToScreenPoint(GameObject.FindGameObjectsWithTag("Player")[i].transform.position);
    
                othMarkerTargetPos.x = Mathf.Clamp(othMarkerTargetPos.x, screenBounds.x, screenBounds.xMax);
                othMarkerTargetPos.y = Mathf.Clamp(othMarkerTargetPos.y, screenBounds.y, screenBounds.yMax);
                GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().mapCamera.GetComponentInChildren<MapCam>().myMarker.transform.position = new Vector3(othMarkerTargetPos.x, othMarkerTargetPos.y, 0);
            }
        }
    }

    public void ScanForPlayers()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            GameObject marker = Instantiate(othMarkerPrefab, transform);
            marker.name = "othMarker_" + i;
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().mapCamera.GetComponentInChildren<MapCam>().myMarker = marker;
        }
    }

    public void DiscardOthPlayerMarkers()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("OthMarker").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("OthMarker")[i]);
        }
    }
}
