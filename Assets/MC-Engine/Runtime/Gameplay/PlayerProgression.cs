using System;
using MCEngine.Inventory;
using MCEngine.World;
using UnityEngine;

namespace MCEngine.Gameplay
{
    public enum ToolTier
    {
        Hand = 0,
        WoodPickaxe = 1,
        StonePickaxe = 2
    }

    [Serializable]
    public sealed class PlayerProgression
    {
        [SerializeField] private ToolTier toolTier;

        public ToolTier ToolTier => toolTier;
        public int MiningPower => (int)toolTier + 1;

        public bool TryCraftWoodPickaxe(BlockInventory inventory)
        {
            return TryUpgrade(inventory, ToolTier.Hand, ToolTier.WoodPickaxe, BlockType.Wood, 8);
        }

        public bool TryCraftStonePickaxe(BlockInventory inventory)
        {
            return TryUpgrade(inventory, ToolTier.WoodPickaxe, ToolTier.StonePickaxe, BlockType.Stone, 15);
        }

        public PlayerProgression Copy()
        {
            Validate();
            return new PlayerProgression
            {
                toolTier = toolTier
            };
        }

        internal void Validate()
        {
            if (!Enum.IsDefined(typeof(ToolTier), toolTier))
            {
                throw new InvalidOperationException($"Unknown tool tier: {toolTier}.");
            }
        }

        private bool TryUpgrade(
            BlockInventory inventory,
            ToolTier requiredTier,
            ToolTier resultTier,
            BlockType material,
            int materialQuantity)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (toolTier != requiredTier || !inventory.TryRemove(material, materialQuantity))
            {
                return false;
            }

            toolTier = resultTier;
            return true;
        }
    }
}
