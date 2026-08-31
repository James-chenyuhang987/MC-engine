using MCEngine.Inventory;
using MCEngine.World;
using NUnit.Framework;

namespace MCEngine.Tests
{
    public sealed class BlockInventoryTests
    {
        [Test]
        public void AddAndRemove_ValidBlocks_TracksQuantity()
        {
            var inventory = new BlockInventory();

            Assert.That(inventory.TryAdd(BlockType.Dirt, 4), Is.True);
            Assert.That(inventory.TryRemove(BlockType.Dirt, 3), Is.True);
            Assert.That(inventory.Count(BlockType.Dirt), Is.EqualTo(1));
        }

        [Test]
        public void Add_WhenStackWouldOverflow_IsAtomic()
        {
            var inventory = new BlockInventory(capacity: 1, maxStackSize: 5);
            inventory.TryAdd(BlockType.Stone, 4);

            Assert.That(inventory.TryAdd(BlockType.Stone, 2), Is.False);
            Assert.That(inventory.Count(BlockType.Stone), Is.EqualTo(4));
        }

        [Test]
        public void Add_WhenDistinctSlotCapacityReached_Fails()
        {
            var inventory = new BlockInventory(capacity: 1);
            inventory.TryAdd(BlockType.Dirt);

            Assert.That(inventory.TryAdd(BlockType.Wood), Is.False);
            Assert.That(inventory.Count(BlockType.Wood), Is.Zero);
        }
    }
}

