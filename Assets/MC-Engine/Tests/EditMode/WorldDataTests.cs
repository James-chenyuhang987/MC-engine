using MCEngine.World;
using NUnit.Framework;
using UnityEngine;

namespace MCEngine.Tests
{
    public sealed class WorldDataTests
    {
        [Test]
        public void Generate_WithSameSeed_ProducesSameWorld()
        {
            var first = WorldGenerator.Generate(12345, 48, 32);
            var second = WorldGenerator.Generate(12345, 48, 32);

            CollectionAssert.AreEqual(first.CopyBlocks(), second.CopyBlocks());
        }

        [Test]
        public void TryMine_ThenTryPlace_UpdatesBlock()
        {
            var world = new WorldData(3, 3, 1);
            var target = new Vector2Int(1, 1);
            world.SetBlock(target, BlockType.Dirt);
            world.SetBlock(Vector2Int.down + target, BlockType.Stone);

            Assert.That(world.TryMine(target, out var minedBlock), Is.True);
            Assert.That(minedBlock, Is.EqualTo(BlockType.Dirt));
            Assert.That(world.GetBlock(target), Is.EqualTo(BlockType.Air));

            Assert.That(world.TryPlace(target, BlockType.Wood), Is.True);
            Assert.That(world.GetBlock(target), Is.EqualTo(BlockType.Wood));
        }

        [Test]
        public void TryMine_Bedrock_DoesNotChangeBlock()
        {
            var world = new WorldData(2, 2, 1);
            var target = Vector2Int.zero;
            world.SetBlock(target, BlockType.Bedrock);

            Assert.That(world.TryMine(target, out _), Is.False);
            Assert.That(world.GetBlock(target), Is.EqualTo(BlockType.Bedrock));
        }
    }
}

