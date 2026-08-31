using System.Collections.Generic;
using MCEngine.Configuration;
using MCEngine.World;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MCEngine.Presentation
{
    public sealed class WorldTilemapView : MonoBehaviour
    {
        private readonly HashSet<Vector2Int> dirtyBlocks = new();
        private readonly Dictionary<BlockType, Tile> tiles = new();
        private GameContentCatalog contentCatalog;
        private Tilemap tilemap;
        private WorldData world;

        public static WorldTilemapView Create(Transform parent, GameContentCatalog catalog)
        {
            var gridObject = new GameObject("World Grid", typeof(Grid));
            gridObject.transform.SetParent(parent);

            var tilemapObject = new GameObject(
                "Blocks",
                typeof(Tilemap),
                typeof(TilemapRenderer),
                typeof(TilemapCollider2D),
                typeof(Rigidbody2D),
                typeof(CompositeCollider2D),
                typeof(WorldTilemapView));
            tilemapObject.transform.SetParent(gridObject.transform);

            var renderer = tilemapObject.GetComponent<TilemapRenderer>();
            renderer.sortingOrder = 0;
            renderer.mode = TilemapRenderer.Mode.Chunk;

            var body = tilemapObject.GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;

            var tileCollider = tilemapObject.GetComponent<TilemapCollider2D>();
            tileCollider.compositeOperation = Collider2D.CompositeOperation.Merge;

            var compositeCollider = tilemapObject.GetComponent<CompositeCollider2D>();
            compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

            var view = tilemapObject.GetComponent<WorldTilemapView>();
            view.contentCatalog = catalog;
            return view;
        }

        public void Render(WorldData sourceWorld)
        {
            world = sourceWorld;
            tilemap ??= GetComponent<Tilemap>();
            EnsurePalette();
            dirtyBlocks.Clear();
            tilemap.ClearAllTiles();

            var bounds = new BoundsInt(0, 0, 0, world.Width, world.Height, 1);
            var renderedTiles = new TileBase[world.Width * world.Height];
            for (var y = 0; y < world.Height; y++)
            {
                for (var x = 0; x < world.Width; x++)
                {
                    var block = world.GetBlock(new Vector2Int(x, y));
                    renderedTiles[y * world.Width + x] = GetTile(block);
                }
            }

            tilemap.SetTilesBlock(bounds, renderedTiles);
            tilemap.CompressBounds();
        }

        public void RefreshBlock(Vector2Int position)
        {
            if (world == null || !world.IsInBounds(position))
            {
                return;
            }

            dirtyBlocks.Add(position);
        }

        public Vector2Int WorldToCell(Vector2 worldPosition)
        {
            tilemap ??= GetComponent<Tilemap>();
            var cell = tilemap.WorldToCell(worldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        private TileBase GetTile(BlockType blockType)
        {
            return blockType == BlockType.Air ? null : tiles[blockType];
        }

        private void EnsurePalette()
        {
            if (tiles.Count > 0)
            {
                return;
            }

            AddTile(BlockType.Grass, new Color(0.28f, 0.65f, 0.25f));
            AddTile(BlockType.Dirt, new Color(0.48f, 0.29f, 0.16f));
            AddTile(BlockType.Stone, new Color(0.43f, 0.45f, 0.49f));
            AddTile(BlockType.Wood, new Color(0.58f, 0.38f, 0.18f));
            AddTile(BlockType.Bedrock, new Color(0.12f, 0.13f, 0.16f));
            AddTile(BlockType.Plank, new Color(0.76f, 0.55f, 0.28f));
            AddTile(BlockType.StoneBrick, new Color(0.32f, 0.34f, 0.38f));
        }

        private void AddTile(BlockType blockType, Color color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = $"{blockType} Tile";
            tile.sprite = contentCatalog?.GetBlockSprite(blockType)
                          ?? RuntimeSpriteFactory.CreateSolidSprite(blockType.ToString(), color);
            tile.color = Color.white;
            tile.colliderType = Tile.ColliderType.Grid;
            tile.hideFlags = HideFlags.HideAndDontSave;
            tiles.Add(blockType, tile);
        }

        private void LateUpdate()
        {
            if (dirtyBlocks.Count == 0)
            {
                return;
            }

            var positions = new Vector3Int[dirtyBlocks.Count];
            var changedTiles = new TileBase[dirtyBlocks.Count];
            var index = 0;
            foreach (var position in dirtyBlocks)
            {
                positions[index] = new Vector3Int(position.x, position.y);
                changedTiles[index] = GetTile(world.GetBlock(position));
                index++;
            }

            tilemap.SetTiles(positions, changedTiles);
            dirtyBlocks.Clear();
        }
    }
}
