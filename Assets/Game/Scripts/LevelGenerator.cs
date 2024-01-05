using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int dungeonWidth = 10;
    public int dungeonHeight = 10;
    public float scale = 20f;
    public int roomScale = 20;

    private void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise((x + transform.position.x) / scale, (y + transform.position.y) / scale);
                int[] randomRotation = {0, 90, -90, 180, -180};
                int randomPrefabNum = Random.Range(0, 6);
                int randomRotationNum = Random.Range(0, 5);

                if (perlinValue > 0.5f)
                {
                    Vector3 roomPosition = new Vector3(x, 0, y) * roomScale;
                    GameObject newRoom = Instantiate(roomPrefabs[randomPrefabNum], roomPosition, Quaternion.identity, transform);
                    newRoom.transform.rotation = Quaternion.Euler(0, randomRotation[randomRotationNum], 0);
                }
            }
        }
    }
}

