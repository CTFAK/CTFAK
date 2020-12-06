using System.Windows.Forms;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.Data;

namespace NetMFAPatcher.GUI
{
    public class ChunkNode:TreeNode
    {
        public ChunkList.Chunk chunk;
        public ChunkLoader loader;

        public ChunkNode(string text, ChunkList.Chunk chunk) : base(text)
        {
            this.loader = chunk.Loader;
            this.chunk = chunk;
        }

        public ChunkNode(string text, ChunkLoader loader) : base(text)
        {
            this.loader = loader;
            this.chunk = loader.Chunk;
        }

        public ChunkNode(string text) : base(text)
        {
        }
    }
}