using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Marching;
using adventure;
public class ChunksManager : MonoBehaviour
{
    [SerializeField]
    private Material terrainMaterial;

    // Start is called before the first frame update
    Vector3 playerPos;
    Dictionary<Vector3Int, Chunk> chunks;
    Dictionary<Vector3Int, MarchingManager> managers;
    Queue<Vector3Int> chunksToAdd;
    ITerrainGenerator tgen = new BasicTerrain();
    void Start()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
        managers = new Dictionary<Vector3Int, MarchingManager>();
        chunksToAdd = new Queue<Vector3Int>();
        
        //QueueNewChunk(new Vector3Int());

        playerPos = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        FindNewChunksToCreate();
        QueueNewChunks();
    }

    private void LateUpdate()
    {
        var toRemove = new List<Vector3Int>();

        foreach (var chunk in managers.Keys)
        {
            var m = managers[chunk];
            if (m.handle.IsCompleted)
            {
                var me = m.GetMesh();
                chunks[chunk].SetMesh(me);
                chunks[chunk].SetMat(terrainMaterial);
                toRemove.Add(chunk);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            managers.Remove(toRemove[i]);
        }
    }

    private void FindNewChunksToCreate()
    {
        var tarPos = playerPos;
        var xMin = (int)playerPos.x - Settings.chunkGenerateDistance;
        var xMax = (int)playerPos.x + Settings.chunkGenerateDistance;
        var yMin = 0;
        var yMax = 1;
        var zMin = (int)playerPos.z - Settings.chunkGenerateDistance;
        var zMax = (int)playerPos.z + Settings.chunkGenerateDistance;
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                for (int z = zMin; z < zMax; z++)
                {
                    var newChunkPos = new Vector3Int(x, y, z);
                    if (!(chunks.ContainsKey(newChunkPos) || chunksToAdd.Contains(newChunkPos)))
                    {
                        chunksToAdd.Enqueue(newChunkPos);
                    }
                }
            }
        }
    }

    private void QueueNewChunks()
    {
        for (int i = 0; i < Settings.maxNewChunksPerFrame; i++)
        {
            if(chunksToAdd.TryDequeue(out Vector3Int newChunk))
            {
                //Check that it hasnt become out of range
                QueueNewChunk(newChunk);
            }
            else
            {
                break;
            }
        }
    }

    private void QueueNewChunk(Vector3Int chunkPos)
    {
        Chunk newChunk = new Chunk(chunkPos);
        
        MarchingManager manager = new MarchingManager();
        var data = tgen.GetData(chunkPos);
        manager.QueueMarching(data);
        chunks.Add(chunkPos, newChunk);
        managers.Add(chunkPos, manager);
    }


}
