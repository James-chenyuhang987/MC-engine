using System;
using System.Collections.Generic;
using MCEngine.World;
using UnityEngine;

namespace MCEngine.Configuration
{
    [Serializable]
    public sealed class BlockVisualDefinition
    {
        [SerializeField] private BlockType blockType;
        [SerializeField] private Sprite sprite;

        public BlockType BlockType => blockType;
        public Sprite Sprite => sprite;
    }

    [CreateAssetMenu(
        fileName = "GameContentCatalog",
        menuName = "MC Engine/Game Content Catalog")]
    public sealed class GameContentCatalog : ScriptableObject
    {
        [SerializeField] private Sprite playerSprite;
        [SerializeField] private List<BlockVisualDefinition> blockVisuals = new();

        public Sprite PlayerSprite => playerSprite;

        public Sprite GetBlockSprite(BlockType blockType)
        {
            var definition = blockVisuals.Find(item => item != null && item.BlockType == blockType);
            return definition?.Sprite;
        }

        private void OnValidate()
        {
            if (blockVisuals == null)
            {
                return;
            }

            var seenBlocks = new HashSet<BlockType>();
            foreach (var visual in blockVisuals)
            {
                if (visual != null && !seenBlocks.Add(visual.BlockType))
                {
                    Debug.LogWarning(
                        $"Game Content Catalog contains more than one {visual.BlockType} entry.",
                        this);
                }
            }
        }
    }
}
