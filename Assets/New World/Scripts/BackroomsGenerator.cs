using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroomsGenerator : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int renderDistance = 3; // Number of chunks to render around the player
    public Vector3 chunkSize = new Vector3(10, 3, 10); // Size of each chunk

    private Transform playerTransform;
    private Dictionary<Vector3, GameObject> chunks = new Dictionary<Vector3, GameObject>();

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdateChunks());
    }

    private IEnumerator UpdateChunks()
    {
        while (true)
        {
            Vector3 playerChunkPosition = GetChunkPosition(playerTransform.position);

            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector3 chunkPosition = new Vector3(
                        playerChunkPosition.x + x * chunkSize.x,
                        0,
                        playerChunkPosition.z + z * chunkSize.z
                    );

                    if (!chunks.ContainsKey(chunkPosition))
                    {
                        GameObject newChunk = Instantiate(segmentPrefab, chunkPosition, Quaternion.identity);
                        RandomlyDeactivateWalls(newChunk);
                        chunks[chunkPosition] = newChunk;
                    }
                    else
                    {
                        chunks[chunkPosition].SetActive(true);
                    }
                }
            }

            List<Vector3> chunksToDeactivate = new List<Vector3>();

            foreach (var chunk in chunks)
            {
                if (Vector3.Distance(playerTransform.position, chunk.Key) > renderDistance * chunkSize.x)
                {
                    chunk.Value.SetActive(false);
                    chunksToDeactivate.Add(chunk.Key);
                }
            }

            foreach (var chunkPos in chunksToDeactivate)
            {
                chunks.Remove(chunkPos);
            }

            yield return new WaitForSeconds(1); // Adjust the update rate as needed
        }
    }

    private Vector3 GetChunkPosition(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkSize.x);
        int chunkZ = Mathf.FloorToInt(position.z / chunkSize.z);

        return new Vector3(chunkX * chunkSize.x, 0, chunkZ * chunkSize.z);
    }

    private void RandomlyDeactivateWalls(GameObject chunk)
    {
        Transform walls = chunk.transform.Find("Walls");
        int numWalls = walls.childCount;
        for (int i = 0; i < numWalls; i++)
        {
            if (Random.value > .3) // Higher chance to deactivate each wall
            {
                walls.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
