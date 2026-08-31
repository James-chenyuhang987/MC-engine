using System;
using UnityEngine;

namespace MCEngine.World
{
    public static class WorldGenerator
    {
        public static WorldData Generate(int seed, int width = 160, int height = 80)
        {
            if (width < 16)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "World width must be at least 16.");
            }

            if (height < 16)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "World height must be at least 16.");
            }

            var world = new WorldData(width, height, seed);
            var random = new System.Random(seed);
            var terrainOffset = random.Next(-100000, 100000);
            var caveOffsetX = random.Next(-100000, 100000);
            var caveOffsetY = random.Next(-100000, 100000);

            for (var x = 0; x < width; x++)
            {
                var terrainNoise = Mathf.PerlinNoise((x + terrainOffset) * 0.045f, 0.5f);
                var surface = Mathf.Clamp(
                    Mathf.RoundToInt(height * 0.48f + (terrainNoise - 0.5f) * height * 0.22f),
                    8,
                    height - 10);

                for (var y = 0; y <= surface; y++)
                {
                    var position = new Vector2Int(x, y);
                    var block = SelectGroundBlock(y, surface);
                    if (CanCarveCave(block, y, surface, caveOffsetX, caveOffsetY, x))
                    {
                        block = BlockType.Air;
                    }

                    world.SetBlock(position, block);
                }

                if (x > 3 && x < width - 4 && random.NextDouble() < 0.055)
                {
                    GrowTree(world, x, surface + 1, random.Next(3, 6));
                }
            }

            return world;
        }

        private static BlockType SelectGroundBlock(int y, int surface)
        {
            if (y == 0)
            {
                return BlockType.Bedrock;
            }

            if (y == surface)
            {
                return BlockType.Grass;
            }

            return y >= surface - 4 ? BlockType.Dirt : BlockType.Stone;
        }

        private static bool CanCarveCave(
            BlockType block,
            int y,
            int surface,
            int offsetX,
            int offsetY,
            int x)
        {
            if (block is BlockType.Bedrock or BlockType.Grass || y >= surface - 3)
            {
                return false;
            }

            var broadNoise = Mathf.PerlinNoise((x + offsetX) * 0.075f, (y + offsetY) * 0.075f);
            var detailNoise = Mathf.PerlinNoise((x - offsetY) * 0.16f, (y + offsetX) * 0.16f);
            return broadNoise > 0.63f && detailNoise > 0.48f;
        }

        private static void GrowTree(WorldData world, int x, int baseY, int trunkHeight)
        {
            for (var y = baseY; y < baseY + trunkHeight && y < world.Height; y++)
            {
                world.SetBlock(new Vector2Int(x, y), BlockType.Wood);
            }
        }
    }
}

