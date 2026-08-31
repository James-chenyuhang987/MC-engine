using System;
using MCEngine.Gameplay;
using MCEngine.Inventory;
using MCEngine.World;
using UnityEngine;

namespace MCEngine.Persistence
{
    [Serializable]
    public sealed class GameSaveData
    {
        [SerializeField] private int schemaVersion = 2;
        [SerializeField] private int worldWidth;
        [SerializeField] private int worldHeight;
        [SerializeField] private int worldSeed;
        [SerializeField] private int[] blocks;
        [SerializeField] private BlockInventory inventory;
        [SerializeField] private PlayerProgression progression;
        [SerializeField] private Vector2 playerPosition;

        public int SchemaVersion => schemaVersion;
        public int WorldWidth => worldWidth;
        public int WorldHeight => worldHeight;
        public int WorldSeed => worldSeed;
        public int[] Blocks => blocks;
        public BlockInventory Inventory => inventory;
        public PlayerProgression Progression => progression;
        public Vector2 PlayerPosition => playerPosition;

        public static GameSaveData Capture(
            WorldData world,
            BlockInventory sourceInventory,
            PlayerProgression sourceProgression,
            Vector2 sourcePlayerPosition)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            if (sourceInventory == null)
            {
                throw new ArgumentNullException(nameof(sourceInventory));
            }

            if (sourceProgression == null)
            {
                throw new ArgumentNullException(nameof(sourceProgression));
            }

            return new GameSaveData
            {
                worldWidth = world.Width,
                worldHeight = world.Height,
                worldSeed = world.Seed,
                blocks = world.CopyBlocks(),
                inventory = sourceInventory.Copy(),
                progression = sourceProgression.Copy(),
                playerPosition = sourcePlayerPosition
            };
        }

        public WorldData RestoreWorld()
        {
            Validate();
            return WorldData.FromBlocks(worldWidth, worldHeight, worldSeed, blocks);
        }

        internal void Validate()
        {
            if (schemaVersion == 1)
            {
                progression = new PlayerProgression();
                schemaVersion = 2;
            }

            if (schemaVersion != 2)
            {
                throw new InvalidOperationException($"Unsupported save schema version: {schemaVersion}.");
            }

            if (worldWidth <= 0
                || worldHeight <= 0
                || blocks == null
                || blocks.LongLength != (long)worldWidth * worldHeight)
            {
                throw new InvalidOperationException("Save data contains invalid world dimensions or blocks.");
            }

            foreach (var block in blocks)
            {
                if (!Enum.IsDefined(typeof(BlockType), block))
                {
                    throw new InvalidOperationException($"Save data contains unknown block type {block}.");
                }
            }

            if (inventory == null)
            {
                throw new InvalidOperationException("Save data does not contain an inventory.");
            }

            inventory.Validate();

            if (progression == null)
            {
                throw new InvalidOperationException("Save data does not contain player progression.");
            }

            progression.Validate();

            if (float.IsNaN(playerPosition.x)
                || float.IsInfinity(playerPosition.x)
                || float.IsNaN(playerPosition.y)
                || float.IsInfinity(playerPosition.y))
            {
                throw new InvalidOperationException("Save data contains an invalid player position.");
            }
        }
    }
}
