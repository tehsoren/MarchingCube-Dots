using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;

namespace adventure
{
    class BasicTerrain : ITerrainGenerator
    {
        public float[] GetData(Vector3Int chunkPos)
        {
            float[] data = new float[Settings.totalDataPoints];
            
            for (int x = 0; x < Settings.dataSize; x++)
            {
                for (int y = 0; y < Settings.dataSize; y++)
                {
                    for (int z = 0; z < Settings.dataSize; z++)
                    {
                        data[Marching.MarchingHelper.PosToIndex(x, y, z)] = UnityEngine.Random.Range(0, 1f);//Mathf.PerlinNoise(x,z);
                    }
                }
            }
            return data;
        }
    }
}
