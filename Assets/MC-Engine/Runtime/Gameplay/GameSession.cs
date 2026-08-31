using System;
using MCEngine.Inventory;
using MCEngine.Persistence;
using MCEngine.Presentation;
using MCEngine.World;
using UnityEngine;

namespace MCEngine.Gameplay
{
    public sealed class GameSession : MonoBehaviour
    {
        private readonly SaveRepository saveRepository = new();
        private Transform player;
        private Collider2D playerCollider;
        private WorldTilemapView worldView;
        private bool initialized;
        private bool hasMiningTarget;
        private Vector2Int miningTarget;
        private int miningDamage;

        public WorldData World { get; private set; }
        public BlockInventory Inventory { get; private set; }
        public PlayerProgression Progression { get; private set; }
        public string StatusMessage { get; private set; } = string.Empty;

        public void Initialize(WorldTilemapView view, Transform playerTransform)
        {
            worldView = view;
            player = playerTransform;
            playerCollider = player.GetComponent<Collider2D>();

            if (saveRepository.HasSave)
            {
                Load();
            }
            else
            {
                CreateNewWorld();
            }

            initialized = true;
        }

        public bool TryMine(Vector2Int position)
        {
            var block = World.GetBlock(position);
            if (!BlockRules.IsMineable(block))
            {
                ResetMining();
                return false;
            }

            if (!Inventory.CanAdd(block))
            {
                SetStatus("Inventory is full.");
                return false;
            }

            if (!hasMiningTarget || miningTarget != position)
            {
                miningTarget = position;
                miningDamage = 0;
                hasMiningTarget = true;
            }

            miningDamage += Progression.MiningPower;
            var hardness = BlockRules.GetHardness(block);
            if (miningDamage < hardness)
            {
                SetStatus($"Mining {block}: {miningDamage}/{hardness}.");
                return true;
            }

            if (!World.TryMine(position, out var minedBlock) || !Inventory.TryAdd(minedBlock))
            {
                throw new InvalidOperationException("Mining state became inconsistent.");
            }

            worldView.RefreshBlock(position);
            ResetMining();
            SetStatus($"Collected {minedBlock}.");
            return true;
        }

        public bool TryPlace(Vector2Int position, BlockType blockType)
        {
            var cellBounds = new Bounds((Vector2)position + Vector2.one * 0.5f, Vector3.one * 0.95f);
            if (cellBounds.Intersects(playerCollider.bounds))
            {
                SetStatus("Cannot place a block inside the player.");
                return false;
            }

            if (!Inventory.TryRemove(blockType))
            {
                SetStatus($"No {blockType} blocks available.");
                return false;
            }

            if (!World.TryPlace(position, blockType))
            {
                Inventory.TryAdd(blockType);
                return false;
            }

            worldView.RefreshBlock(position);
            ResetMining();
            SetStatus($"Placed {blockType}.");
            return true;
        }

        public void TryCraftPlanks()
        {
            SetStatus(CraftingService.TryCraftPlanks(Inventory)
                ? "Crafted 4 Planks from 1 Wood."
                : "Crafting Planks requires 1 Wood and inventory space.");
        }

        public void TryCraftStoneBricks()
        {
            SetStatus(CraftingService.TryCraftStoneBricks(Inventory)
                ? "Crafted 2 Stone Bricks from 2 Stone."
                : "Crafting Stone Bricks requires 2 Stone and inventory space.");
        }

        public void TryCraftWoodPickaxe()
        {
            SetStatus(Progression.TryCraftWoodPickaxe(Inventory)
                ? "Crafted a Wood Pickaxe. Mining power increased to 2."
                : "A Wood Pickaxe requires 8 Wood and the Hand tool tier.");
        }

        public void TryCraftStonePickaxe()
        {
            SetStatus(Progression.TryCraftStonePickaxe(Inventory)
                ? "Crafted a Stone Pickaxe. Mining power increased to 3."
                : "A Stone Pickaxe requires a Wood Pickaxe and 15 Stone.");
        }

        public void CancelMining()
        {
            ResetMining();
        }

        public void Save()
        {
            if (!initialized)
            {
                return;
            }

            saveRepository.Save(GameSaveData.Capture(World, Inventory, Progression, player.position));
            SetStatus($"World saved to {saveRepository.SavePath}.");
        }

        public void Load()
        {
            var saveData = saveRepository.Load();
            World = saveData.RestoreWorld();
            Inventory = saveData.Inventory.Copy();
            Progression = saveData.Progression.Copy();
            worldView.Render(World);
            player.position = saveData.PlayerPosition;
            ResetMining();
            SetStatus("Saved world loaded.");
        }

        public void CreateNewWorld()
        {
            var seed = unchecked((int)DateTime.UtcNow.Ticks);
            World = WorldGenerator.Generate(seed);
            Inventory = new BlockInventory();
            Progression = new PlayerProgression();
            Inventory.TryAdd(BlockType.Dirt, 25);
            worldView.Render(World);

            var spawnX = World.Width / 2;
            var spawnY = World.FindSurfaceHeight(spawnX) + 2.5f;
            player.position = new Vector2(spawnX + 0.5f, spawnY);
            ResetMining();
            SetStatus($"Created world with seed {seed}.");
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void SetStatus(string message)
        {
            StatusMessage = message;
        }

        private void ResetMining()
        {
            hasMiningTarget = false;
            miningDamage = 0;
        }
    }
}
