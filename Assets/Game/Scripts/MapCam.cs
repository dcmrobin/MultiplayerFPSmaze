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
    public GameObject ventMarkerPrefab;
    bool showingVentMarkers;

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
    }

    public void ScanForPlayers()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            GameObject marker = Instantiate(othMarkerPrefab, transform.Find("Canvas"));
            marker.name = "othMarker_" + i;
            marker.GetComponent<OthMarker>().target = GameObject.FindGameObjectsWithTag("Player")[i].transform;
        }
    }

    public void DiscardOthPlayerMarkers()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("OthMarker").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("OthMarker")[i]);
        }
    }

    public void ToggleVentMarkers()
    {
        if (!showingVentMarkers)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Vent").Length; i++)
            {
                GameObject marker = Instantiate(ventMarkerPrefab, transform.Find("Canvas"));
                marker.name = "ventMarker_" + i;
                marker.GetComponent<OthMarker>().target = GameObject.FindGameObjectsWithTag("Vent")[i].transform;
            }
            showingVentMarkers = true;
        }
        else
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("VentMarker").Length; i++)
            {
                Destroy(GameObject.FindGameObjectsWithTag("VentMarker")[i]);
            }
            showingVentMarkers = false;
        }
    }
}
