using MCEngine.Inventory;
using MCEngine.Gameplay;
using MCEngine.Persistence;
using MCEngine.World;
using NUnit.Framework;
using UnityEngine;

namespace MCEngine.Tests
{
    public sealed class GameSaveCodecTests
    {
        [Test]
        public void SerializeThenDeserialize_RestoresGameState()
        {
            var world = new WorldData(4, 3, 777);
            world.SetBlock(new Vector2Int(2, 1), BlockType.Stone);
            var inventory = new BlockInventory();
            inventory.TryAdd(BlockType.Wood, 6);
            var progression = new PlayerProgression();
            var playerPosition = new Vector2(2.5f, 8f);
            var source = GameSaveData.Capture(world, inventory, progression, playerPosition);

            var restoredSave = GameSaveCodec.Deserialize(GameSaveCodec.Serialize(source));
            var restoredWorld = restoredSave.RestoreWorld();

            Assert.That(restoredWorld.Seed, Is.EqualTo(777));
            Assert.That(restoredWorld.GetBlock(new Vector2Int(2, 1)), Is.EqualTo(BlockType.Stone));
            Assert.That(restoredSave.Inventory.Count(BlockType.Wood), Is.EqualTo(6));
            Assert.That(restoredSave.Progression.ToolTier, Is.EqualTo(ToolTier.Hand));
            Assert.That(restoredSave.PlayerPosition, Is.EqualTo(playerPosition));
        }

        [Test]
        public void Deserialize_WithInvalidDimensions_Throws()
        {
            const string invalidSave = "{\"schemaVersion\":1,\"worldWidth\":4,\"worldHeight\":3,\"blocks\":[1]}";

            Assert.That(
                () => GameSaveCodec.Deserialize(invalidSave),
                Throws.TypeOf<System.InvalidOperationException>());
        }

        [Test]
        public void Deserialize_VersionOneSave_MigratesProgression()
        {
            const string versionOneSave =
                "{\"schemaVersion\":1,\"worldWidth\":1,\"worldHeight\":1,\"worldSeed\":9,"
                + "\"blocks\":[1],\"inventory\":{\"capacity\":8,\"maxStackSize\":999,\"slots\":[]},"
                + "\"playerPosition\":{\"x\":0.5,\"y\":2.0}}";

            var migrated = GameSaveCodec.Deserialize(versionOneSave);

            Assert.That(migrated.SchemaVersion, Is.EqualTo(2));
            Assert.That(migrated.Progression.ToolTier, Is.EqualTo(ToolTier.Hand));
        }
    }
}
