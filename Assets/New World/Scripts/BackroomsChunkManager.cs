using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroomsChunkManager : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int renderDistance = 3; // Number of chunks to render around the player
    public Vector3 chunkSize = new Vector3(10, 3, 10); // Size of each chunk
    public ObjectPool objectPool;

    private Transform playerTransform;
    private Dictionary<Vector3, GameObject> activeChunks = new Dictionary<Vector3, GameObject>();
    private HashSet<Vector3> allChunks = new HashSet<Vector3>();

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

            // Activate necessary chunks
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector3 chunkPosition = new Vector3(
                        playerChunkPosition.x + x * chunkSize.x,
                        transform.position.y,
                        playerChunkPosition.z + z * chunkSize.z
                    );

                    if (!activeChunks.ContainsKey(chunkPosition))
                    {
                        GameObject newChunk = objectPool.GetObject(chunkPosition, Quaternion.identity);
                        RandomlyDeactivateWalls(newChunk);
                        activeChunks[chunkPosition] = newChunk;
                        allChunks.Add(chunkPosition);
                    }
                }
            }

            // Deactivate chunks out of range
            List<Vector3> chunksToDeactivate = new List<Vector3>();
            foreach (var chunk in activeChunks)
            {
                if (Vector3.Distance(playerTransform.position, chunk.Key) > renderDistance * chunkSize.x)
                {
                    objectPool.ReturnObject(chunk.Value);
                    chunksToDeactivate.Add(chunk.Key);
                }
            }

            foreach (var chunkPos in chunksToDeactivate)
            {
                activeChunks.Remove(chunkPos);
            }

            yield return new WaitForSeconds(0.5f); // Adjust the update rate as needed
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
        int randWall = Random.Range(0, 3);
        if (Random.value > 0.3)
        {
            walls.GetChild(randWall).gameObject.SetActive(true);
        }

        if (Random.value > 0.6)// Chance to spawn without a light
        {
            walls.parent.Find("Ceiling").GetChild(0).gameObject.SetActive(false);
        }

        if (Random.value > 0.993)
        {
            walls.parent.Find("Floor").GetComponent<BoxCollider>().isTrigger = true;
        }
    }
}
