using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace adventure
{
    interface ITerrainGenerator
    {
        public float[] GetData(Vector3Int chunkPos);
    }
}
