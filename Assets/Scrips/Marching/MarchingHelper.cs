using adventure;
using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

namespace Marching
{


    public static class MarchingHelper
    {
        public static int PosToIndex(int x, int y, int z)
        {
            return x + Settings.dataSize * (y + Settings.dataSize * z);
        }
        public static int PosToIndex(int3 pos)
        {
            return pos.x + Settings.dataSize * (pos.y + Settings.dataSize * pos.z);
        }
        public static int GetCubeIndex(NativeArray<float> values, float isoLevel)
        {
            int cubeIndex = 0;
            if (values[0] < isoLevel) cubeIndex |= 1;
            if (values[1] < isoLevel) cubeIndex |= 2;
            if (values[2] < isoLevel) cubeIndex |= 4;
            if (values[3] < isoLevel) cubeIndex |= 8;
            if (values[4] < isoLevel) cubeIndex |= 16;
            if (values[5] < isoLevel) cubeIndex |= 32;
            if (values[6] < isoLevel) cubeIndex |= 64;
            if (values[7] < isoLevel) cubeIndex |= 128;

            return cubeIndex;
        }
        public static int3 GetCornerVertex(int3 pos, int cornerIndex)
        {
            return GetCornerVertex(pos.x, pos.y, pos.z, cornerIndex);
        }
        public static int3 GetCornerVertex(int x,int y,int z, int cornerIndex)
        {
            switch (cornerIndex)
            {
                case 0:
                    return new int3(x, y, z);
                case 1:
                    return new int3(x + 1, y, z);
                case 2:
                    return new int3(x + 1, y + 1, z);
                case 3:
                    return new int3(x, y + 1, z);
                case 4:
                    return new int3(x, y, z + 1);
                case 5:
                    return new int3(x + 1, y, z + 1);
                case 6:
                    return new int3(x + 1, y + 1, z + 1);
                case 7:
                    return new int3(x, y + 1, z + 1);
                default:
                    throw new Exception();
            }
        }
        public static int GetCornerVertexIndex(int x, int y, int z, int cornerIndex)//
        {
            return PosToIndex(GetCornerVertex(x, y, z, cornerIndex));
        }
        public static int GetCornerVertexIndex(int3 cubeIndex,int cornerIndex)
        {
            return GetCornerVertexIndex(cubeIndex.x, cubeIndex.y, cubeIndex.z, cornerIndex);
        }
        public static float3 MidpointEdge(float3 p1, float3 p2)
        {
            var a = (p1 + p2) / 2;
            return a;
        }
        public static float3 InterpolateEdge(float3 p1, float3 p2, float v1, float v2, float isoLevel)
        {
            var pDiff = p2 - p1;
            var vDiff = v2 - v1;
            var p = p1 + (isoLevel - v1) * pDiff / vDiff;
            return p;
        }

    }
}