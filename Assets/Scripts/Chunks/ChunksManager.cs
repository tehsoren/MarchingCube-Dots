using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Marching;
using adventure;
using UnityEngine.Profiling;
public class ChunksManager : MonoBehaviour
{
    [SerializeField]
    private Material terrainMaterial;
    [SerializeField]
    GameObject player;
    Vector3Int playerChunk;
    
    Dictionary<Vector3Int, Chunk> chunks;
    Dictionary<Vector3Int, MarchingManager> managers;
    Queue<Vector3Int> chunksToAdd;
    ITerrainGenerator tgen = new BasicTerrain();
    void Start()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
        managers = new Dictionary<Vector3Int, MarchingManager>();
        chunksToAdd = new Queue<Vector3Int>();
        //playerChunk = PlayerChunkPosInt();
        FindNewChunksToCreate();
    }

    // Update is called once per frame
    void Update()
    {
        if(!(playerChunk == PlayerChunkPosInt()))
        {
            playerChunk = PlayerChunkPosInt();
            FindNewChunksToCreate();
            RemoveOldChunks();
            //SetChunksActiveStatus();

        }
        Profiler.BeginSample("que");
        QueueNewChunks();
        Profiler.EndSample();


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
        var tarPos = PlayerChunkPosInt();
        var xMin = tarPos.x - Settings.chunkMaxDistance;
        var xMax = tarPos.x + Settings.chunkMaxDistance;
        var yMin = Settings.minChunkHeight;
        var yMax = Settings.maxChunkHeight;
        var zMin = tarPos.z - Settings.chunkMaxDistance;
        var zMax = tarPos.z + Settings.chunkMaxDistance;
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

    private void RemoveOldChunks()
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (var key in chunks.Keys)
        {
            if((chunks[key].chunkPos-PlayerChunkPosInt()).magnitude > Settings.chunkMaxDistance+2)
            {
                if(!managers.ContainsKey(key))
                {
                    toRemove.Add(key);
                }
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            chunks[toRemove[i]].DestroyGameObject();
            chunks.Remove(toRemove[i]);
            managers.Remove(toRemove[i]);
        }
    }
    private void SetChunksActiveStatus()
    {
        foreach (var key in chunks.Keys)
        {
            var chunk = chunks[key];
            var dist = (chunk.chunkPos - PlayerChunkPosInt()).magnitude;
            var status = (dist < Settings.chunkViewDistance );
            chunk.SetActive(status);            
        }
    }
    private void QueueNewChunks()
    {
        for (int i = 0; i < Settings.maxNewChunksPerFrame; i++)
        {
            if(chunksToAdd.TryDequeue(out Vector3Int newChunk))
            {
                //Check that it hasnt become out of range
                if((newChunk-PlayerChunkPosInt()).magnitude > Settings.chunkMaxDistance)
                    i -= 1;
                else 
                    QueueNewChunk(newChunk);
            }
            else
                break;
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

    private Vector3 PlayerChunkPos()
    {
        return (player.transform.position / Settings.chunkSize/Settings.gridSize);
    }
    private Vector3Int PlayerChunkPosInt()
    {
        var pos = PlayerChunkPos();
        return new Vector3Int((int)pos.x,(int)pos.y,(int)pos.z);
    }
}
