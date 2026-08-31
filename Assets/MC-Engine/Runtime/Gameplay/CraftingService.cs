using System;
using MCEngine.Inventory;
using MCEngine.World;

namespace MCEngine.Gameplay
{
    public static class CraftingService
    {
        public static bool TryCraftPlanks(BlockInventory inventory)
        {
            return TryExchange(inventory, BlockType.Wood, 1, BlockType.Plank, 4);
        }

        public static bool TryCraftStoneBricks(BlockInventory inventory)
        {
            return TryExchange(inventory, BlockType.Stone, 2, BlockType.StoneBrick, 2);
        }

        private static bool TryExchange(
            BlockInventory inventory,
            BlockType input,
            int inputQuantity,
            BlockType output,
            int outputQuantity)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (!inventory.TryRemove(input, inputQuantity))
            {
                return false;
            }

            if (inventory.TryAdd(output, outputQuantity))
            {
                return true;
            }

            if (!inventory.TryAdd(input, inputQuantity))
            {
                throw new InvalidOperationException("Crafting rollback failed.");
            }

            return false;
        }
    }
}
