namespace adventure
{
    public static class Settings
    {
        //Individual Chunk settings
        public static readonly int chunkSize = 16;
        public static readonly int dataSize = chunkSize + 1;
        public static readonly int totalDataPoints = dataSize * dataSize * dataSize;
        public static readonly float gridSize = 1;

        //General Chunk settings
        public static readonly int maxNewChunksPerFrame = 2;
        public static readonly int chunkBufferDistance = 1;
        public static readonly int chunkViewDistance = 1;
        public static readonly int chunkGenerateDistance = chunkBufferDistance + chunkViewDistance;


        //Marching Settings
        public static readonly float isoLevel = 0.5f; // 0 is in, 1 is out
    }
}

