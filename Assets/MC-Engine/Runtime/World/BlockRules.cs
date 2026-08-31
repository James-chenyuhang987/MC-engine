using System;

namespace MCEngine.World
{
    public static class BlockRules
    {
        public static int GetHardness(BlockType blockType)
        {
            return blockType switch
            {
                BlockType.Air => 0,
                BlockType.Grass => 1,
                BlockType.Dirt => 1,
                BlockType.Wood => 2,
                BlockType.Plank => 1,
                BlockType.Stone => 3,
                BlockType.StoneBrick => 4,
                BlockType.Bedrock => int.MaxValue,
                _ => throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null)
            };
        }

        public static bool IsMineable(BlockType blockType)
        {
            return blockType is not BlockType.Air and not BlockType.Bedrock;
        }
    }
}
