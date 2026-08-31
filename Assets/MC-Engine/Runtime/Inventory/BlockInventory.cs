using System;
using System.Collections.Generic;
using MCEngine.World;
using UnityEngine;

namespace MCEngine.Inventory
{
    [Serializable]
    public sealed class InventorySlot
    {
        [SerializeField] private BlockType blockType;
        [SerializeField] private int quantity;

        public InventorySlot(BlockType blockType, int quantity)
        {
            this.blockType = blockType;
            this.quantity = quantity;
        }

        public BlockType BlockType => blockType;
        public int Quantity => quantity;

        internal void Add(int amount)
        {
            quantity += amount;
        }
    }

    [Serializable]
    public sealed class BlockInventory
    {
        [SerializeField] private int capacity;
        [SerializeField] private int maxStackSize;
        [SerializeField] private List<InventorySlot> slots = new();

        public BlockInventory(int capacity = 8, int maxStackSize = 999)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            if (maxStackSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxStackSize));
            }

            this.capacity = capacity;
            this.maxStackSize = maxStackSize;
        }

        public IReadOnlyList<InventorySlot> Slots => slots;

        public int Count(BlockType blockType)
        {
            var slot = FindSlot(blockType);
            return slot?.Quantity ?? 0;
        }

        public bool CanAdd(BlockType blockType, int quantity = 1)
        {
            if (!IsStorable(blockType) || quantity <= 0)
            {
                return false;
            }

            var slot = FindSlot(blockType);
            return slot != null
                ? slot.Quantity + quantity <= maxStackSize
                : slots.Count < capacity && quantity <= maxStackSize;
        }

        public bool CanRemove(BlockType blockType, int quantity = 1)
        {
            return quantity > 0 && Count(blockType) >= quantity;
        }

        public bool TryAdd(BlockType blockType, int quantity = 1)
        {
            if (!CanAdd(blockType, quantity))
            {
                return false;
            }

            var slot = FindSlot(blockType);
            if (slot != null)
            {
                slot.Add(quantity);
                return true;
            }

            slots.Add(new InventorySlot(blockType, quantity));
            return true;
        }

        public bool TryRemove(BlockType blockType, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return false;
            }

            var slot = FindSlot(blockType);
            if (slot == null || slot.Quantity < quantity)
            {
                return false;
            }

            slot.Add(-quantity);
            if (slot.Quantity == 0)
            {
                slots.Remove(slot);
            }

            return true;
        }

        public BlockInventory Copy()
        {
            Validate();
            var copy = new BlockInventory(capacity, maxStackSize);
            foreach (var slot in slots)
            {
                copy.slots.Add(new InventorySlot(slot.BlockType, slot.Quantity));
            }

            return copy;
        }

        internal void Validate()
        {
            if (capacity <= 0 || maxStackSize <= 0 || slots == null || slots.Count > capacity)
            {
                throw new InvalidOperationException("Inventory settings or slot count are invalid.");
            }

            var blockTypes = new HashSet<BlockType>();
            foreach (var slot in slots)
            {
                if (slot == null
                    || slot.BlockType is BlockType.Air or BlockType.Bedrock
                    || !Enum.IsDefined(typeof(BlockType), slot.BlockType)
                    || slot.Quantity <= 0
                    || slot.Quantity > maxStackSize
                    || !blockTypes.Add(slot.BlockType))
                {
                    throw new InvalidOperationException("Inventory contains an invalid slot.");
                }
            }
        }

        private InventorySlot FindSlot(BlockType blockType)
        {
            return slots.Find(slot => slot.BlockType == blockType);
        }

        private static bool IsStorable(BlockType blockType)
        {
            return blockType is not BlockType.Air and not BlockType.Bedrock
                   && Enum.IsDefined(typeof(BlockType), blockType);
        }
    }
}
