using System;
using UnityEngine;

namespace MCEngine.World
{
    [Serializable]
    public sealed class WorldData
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int seed;
        [SerializeField] private int[] blocks;

        public WorldData(int width, int height, int seed)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if ((long)width * height > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "World dimensions are too large.");
            }

            this.width = width;
            this.height = height;
            this.seed = seed;
            blocks = new int[width * height];
        }

        public int Width => width;
        public int Height => height;
        public int Seed => seed;

        public bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0
                   && position.x < width
                   && position.y >= 0
                   && position.y < height;
        }

        public BlockType GetBlock(Vector2Int position)
        {
            return IsInBounds(position) ? (BlockType)blocks[GetIndex(position)] : BlockType.Bedrock;
        }

        public void SetBlock(Vector2Int position, BlockType blockType)
        {
            if (!IsInBounds(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            blocks[GetIndex(position)] = (int)blockType;
        }

        public bool TryMine(Vector2Int position, out BlockType minedBlock)
        {
            minedBlock = GetBlock(position);
            if (minedBlock is BlockType.Air or BlockType.Bedrock)
            {
                return false;
            }

            SetBlock(position, BlockType.Air);
            return true;
        }

        public bool TryPlace(Vector2Int position, BlockType blockType)
        {
            if (!IsInBounds(position)
                || blockType is BlockType.Air or BlockType.Bedrock
                || !Enum.IsDefined(typeof(BlockType), blockType)
                || GetBlock(position) != BlockType.Air
                || !HasSolidNeighbour(position))
            {
                return false;
            }

            SetBlock(position, blockType);
            return true;
        }

        public int FindSurfaceHeight(int x)
        {
            if (x < 0 || x >= width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            for (var y = height - 1; y >= 0; y--)
            {
                if (GetBlock(new Vector2Int(x, y)) != BlockType.Air)
                {
                    return y;
                }
            }

            return 0;
        }

        public int[] CopyBlocks()
        {
            return (int[])blocks.Clone();
        }

        public static WorldData FromBlocks(int width, int height, int seed, int[] sourceBlocks)
        {
            if (sourceBlocks == null)
            {
                throw new ArgumentNullException(nameof(sourceBlocks));
            }

            if (sourceBlocks.LongLength != (long)width * height)
            {
                throw new ArgumentException("Block count does not match the world dimensions.", nameof(sourceBlocks));
            }

            var world = new WorldData(width, height, seed);
            world.blocks = (int[])sourceBlocks.Clone();
            return world;
        }

        private bool HasSolidNeighbour(Vector2Int position)
        {
            return IsSolid(position + Vector2Int.left)
                   || IsSolid(position + Vector2Int.right)
                   || IsSolid(position + Vector2Int.up)
                   || IsSolid(position + Vector2Int.down);
        }

        private bool IsSolid(Vector2Int position)
        {
            return IsInBounds(position) && GetBlock(position) != BlockType.Air;
        }

        private int GetIndex(Vector2Int position)
        {
            return position.y * width + position.x;
        }
    }
}
