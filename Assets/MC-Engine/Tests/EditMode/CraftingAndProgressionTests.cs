using MCEngine.Gameplay;
using MCEngine.Inventory;
using MCEngine.World;
using NUnit.Framework;

namespace MCEngine.Tests
{
    public sealed class CraftingAndProgressionTests
    {
        [Test]
        public void CraftPlanks_WithWood_ExchangesMaterials()
        {
            var inventory = new BlockInventory();
            inventory.TryAdd(BlockType.Wood, 2);

            Assert.That(CraftingService.TryCraftPlanks(inventory), Is.True);
            Assert.That(inventory.Count(BlockType.Wood), Is.EqualTo(1));
            Assert.That(inventory.Count(BlockType.Plank), Is.EqualTo(4));
        }

        [Test]
        public void CraftPlanks_WithoutOutputSlot_RollsBackInput()
        {
            var inventory = new BlockInventory(capacity: 1);
            inventory.TryAdd(BlockType.Wood, 2);

            Assert.That(CraftingService.TryCraftPlanks(inventory), Is.False);
            Assert.That(inventory.Count(BlockType.Wood), Is.EqualTo(2));
            Assert.That(inventory.Count(BlockType.Plank), Is.Zero);
        }

        [Test]
        public void ToolProgression_ConsumesMaterialsAndIncreasesMiningPower()
        {
            var inventory = new BlockInventory();
            inventory.TryAdd(BlockType.Wood, 8);
            inventory.TryAdd(BlockType.Stone, 15);
            var progression = new PlayerProgression();

            Assert.That(progression.TryCraftWoodPickaxe(inventory), Is.True);
            Assert.That(progression.TryCraftStonePickaxe(inventory), Is.True);
            Assert.That(progression.ToolTier, Is.EqualTo(ToolTier.StonePickaxe));
            Assert.That(progression.MiningPower, Is.EqualTo(3));
            Assert.That(inventory.Count(BlockType.Wood), Is.Zero);
            Assert.That(inventory.Count(BlockType.Stone), Is.Zero);
        }

        [TestCase(BlockType.Dirt, 1)]
        [TestCase(BlockType.Wood, 2)]
        [TestCase(BlockType.Stone, 3)]
        [TestCase(BlockType.StoneBrick, 4)]
        public void BlockHardness_UsesExpectedProgression(BlockType blockType, int expectedHardness)
        {
            Assert.That(BlockRules.GetHardness(blockType), Is.EqualTo(expectedHardness));
        }
    }
}
