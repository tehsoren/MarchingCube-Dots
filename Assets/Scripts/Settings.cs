namespace adventure
{
    public static class Settings
    {
        //Individual Chunk settings
        public static readonly int chunkSize = 8;
        public static readonly int dataSize = chunkSize + 1;
        public static readonly int totalDataPoints = dataSize * dataSize * dataSize;
        public static readonly float gridSize = 2;//how far to go before sample

        //General Chunk settings
        public static readonly int maxNewChunksPerFrame = 10;
        public static readonly int chunkBufferDistance = 0;
        public static readonly int chunkViewDistance = 6;
        public static readonly int chunkMaxDistance = chunkBufferDistance + chunkViewDistance;
        public static readonly int maxChunkHeight = 5;
        public static readonly int minChunkHeight = 0;

        //Marching Settings
        public static readonly float isoLevel = 0.5f; // 0 is in, 1 is out
    }
}

