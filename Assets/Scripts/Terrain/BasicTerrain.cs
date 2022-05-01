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
        FastNoiseLite fnl;
        public BasicTerrain()
        {
            fnl = new FastNoiseLite();
        }
        public float[] GetData(Vector3Int chunkPos)
        {
            float[] data = new float[Settings.totalDataPoints];
            
            for (int x = 0; x < Settings.dataSize; x++)
            {
                var nx = (chunkPos.x * Settings.chunkSize + x) * Settings.gridSize;
                for (int y = 0; y < Settings.dataSize; y++)
                {
                    var ny = (chunkPos.y * Settings.chunkSize + y) * Settings.gridSize;
                    for (int z = 0; z < Settings.dataSize; z++)
                    {
                        var nz = (chunkPos.z * Settings.chunkSize + z) * Settings.gridSize;
                        var noise = (fnl.GetNoise(nx*10,ny*10,nz*10)+1)/2;
                        data[Marching.MarchingHelper.PosToIndex(x, y, z)] = noise;
                    }
                }
            }
            return data;
        }
    }
}
