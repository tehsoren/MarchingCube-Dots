using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using adventure;

namespace Marching
{
    public class MarchingManager 
    {
        public JobHandle handle;
        MarchingJob job;
        public void QueueMarching(float[] data)
        {
            job = new MarchingJob
            {
                data = new NativeArray<float>(data, Allocator.TempJob),
                vertices = new NativeList<float3>(500, Allocator.TempJob),
                triangles = new NativeList<int>(200, Allocator.TempJob),
                presentVertices = new NativeHashMap<MarchingJob.LSF3, int>(200, Allocator.TempJob),
        };

            handle = job.Schedule();
        }
        public void CleanJob()
        {
            job.vertices.Dispose();
            job.triangles.Dispose();
            job.data.Dispose();
            job.presentVertices.Dispose();
        }
        public Mesh GetMesh()
        {
            handle.Complete();
            Vector3[] verts = new Vector3[job.vertices.Length];
            int[] triangles = new int[job.triangles.Length];
            for (int i = 0; i < job.vertices.Length; i++)
            {
                verts[i] = job.vertices[i];
            }
            for (int i = 0; i < job.triangles.Length; i++)
            {
                triangles[i] = job.triangles[i];
            }
            Mesh newMesh = new Mesh
            {
                vertices = verts,
                triangles = triangles
            };
            newMesh.RecalculateNormals();
            newMesh.RecalculateTangents();

            CleanJob();
            return newMesh;
        }
    }
}
