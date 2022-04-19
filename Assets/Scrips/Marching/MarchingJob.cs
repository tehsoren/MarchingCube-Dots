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
        public NativeHashMap<int, int> presentVertices;
        private int counter;
         //public NativeHashMap<int, int> verticesMapping;//key is 2 corners, to index in vertices
        public void Execute()
        {
            
            counter = 0;
            //verticesMapping = new NativeHashMap<int, int>();
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
            var hash = SimpleHashFunc(cornera, cornerb);
            if(presentVertices.ContainsKey(hash))
            {
                var index = presentVertices[hash];
                triangles.Add(index);
            }
            else 
            {
                var corneraval = cubeData[cubeVertexIndexA];
                var cornerbval = cubeData[cubeVertexIndexB];
                var newVertex = MarchingHelper.InterpolateEdge(cornera, cornerb, corneraval, cornerbval, Settings.isoLevel);
                vertices.Add(newVertex * Settings.gridSize);
                triangles.Add(counter);
                presentVertices.Add(hash, counter);
                counter += 1;
                
            }



        }


        private int SimpleHashFunc(int3 cornera, int3 cornerb)
        {
            var a = MarchingHelper.PosToIndex(cornera);
            var b = MarchingHelper.PosToIndex(cornerb);
            b *= Settings.totalDataPoints;
            return a + b;
        }


    }
}