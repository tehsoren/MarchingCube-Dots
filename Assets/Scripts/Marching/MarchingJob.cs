using adventure;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

namespace Marching
{
    [BurstCompile]
    public struct MarchingJob : IJob
    {
        [ReadOnly] public NativeArray<float> data;
        [WriteOnly] public NativeList<float3> vertices;
        [WriteOnly] public NativeList<int> triangles;
        public NativeHashMap<LSF3, int> presentVertices;
        private int counter;
        public void Execute()
        {
            
            counter = 0;
            for (int x = 0; x < Settings.chunkSize; x++)
            {
                for (int y = 0; y < Settings.chunkSize; y++)
                {
                    for (int z = 0; z < Settings.chunkSize; z++)
                    {
                        NativeArray<float> cubeData = GetCubeData(data,x, y, z);
                        // calc triangulation index
                        int triIndex = MarchingHelper.GetCubeIndex(cubeData, Settings.isoLevel);
                        //find triangles
                        FindTriangles(new int3(x,y,z),triIndex, cubeData);
                        cubeData.Dispose();
                    }
                }
            }
        }

        private static NativeArray<float> GetCubeData(NativeArray<float> data,int x, int y, int z)
        {
            NativeArray<float> cubeData = new NativeArray<float>(8, Allocator.Temp);
            cubeData[0] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 0)];
            cubeData[1] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 1)];
            cubeData[2] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 2)];
            cubeData[3] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 3)];
            cubeData[4] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 4)];
            cubeData[5] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 5)];
            cubeData[6] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 6)];
            cubeData[7] = data[MarchingHelper.GetCornerVertexIndex(x, y, z, 7)];
            return cubeData;
        }
        private void FindTriangles(int3 cubeIndex, int triIndex, NativeArray<float> cubeData)
        {
            for (int i = 0; MarchingTables.TriangleTable[triIndex][i] > -1; i += 3)
            {
                int index1 = MarchingTables.TriangleTable[triIndex][i];
                TryAddNew(cubeIndex, index1, cubeData);
                int index2 = MarchingTables.TriangleTable[triIndex][i + 1];
                TryAddNew(cubeIndex, index2, cubeData);
                int index3 = MarchingTables.TriangleTable[triIndex][ i + 2];
                TryAddNew(cubeIndex, index3, cubeData);
            }
        }

        private void TryAddNew(int3 cubepos,int triIndex, NativeArray<float> cubeData)
        {
            int cubeVertexIndexA = MarchingTables.EdgeVertexTableA[triIndex];
            int cubeVertexIndexB = MarchingTables.EdgeVertexTableB[triIndex];
            var cornera = MarchingHelper.GetCornerVertex(cubepos, cubeVertexIndexA);
            var cornerb = MarchingHelper.GetCornerVertex(cubepos, cubeVertexIndexB);
            var corneraval = cubeData[cubeVertexIndexA];
            var cornerbval = cubeData[cubeVertexIndexB];
            var newVertex = MarchingHelper.InterpolateEdge(cornera, cornerb, corneraval, cornerbval, Settings.isoLevel);
            if (presentVertices.ContainsKey(new LSF3(newVertex)))
            {
                var index = presentVertices[new LSF3(newVertex)];
                triangles.Add(index);
            }
            else
            {
                vertices.Add(newVertex * Settings.gridSize);
                triangles.Add(counter);
                presentVertices.Add(new LSF3(newVertex), counter);
                counter += 1;
            }
        }

        public struct LSF3 : System.IEquatable<LSF3>//Less strict float3
        {
            public float3 val;

            public LSF3(float3 val)
            {
                this.val = val;
            }

            public bool Equals(LSF3 other)
            {
                var t = (val - other.val);
                return (math.abs(t.x) + math.abs(t.y) + math.abs(t.z)) < 0.001;
            }
            public override int GetHashCode()
            {
                var i3 = new int3((int)val.x, (int)val.y, (int)val.z);
                return (int)math.hash(i3);
            }
        }
    }
}