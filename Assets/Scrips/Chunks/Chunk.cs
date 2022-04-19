using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using adventure;
using Marching;

namespace adventure
{
    public class Chunk
    {
        public Vector3Int chunkPos;
        MeshFilter mf;
        MeshRenderer mr;
        GameObject gameObject;

        public Chunk(Vector3Int chunkPos)
        {
            this.chunkPos = chunkPos;
            GenerateGameobject();
        }

        private void GenerateGameobject()
        {
            GameObject go = new GameObject("Chunk: " + chunkPos.ToString());
            go.transform.position = (Vector3)chunkPos * Settings.gridSize*Settings.chunkSize;
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();
        }

        public void SetMesh(Mesh mesh)
        {
            mf.mesh = mesh;
        }
        public void SetMat(Material material)
        {
            mr.material = material;
        }
    }

}
