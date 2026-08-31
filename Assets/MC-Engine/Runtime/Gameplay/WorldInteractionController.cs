using MCEngine.Player;
using MCEngine.Presentation;
using MCEngine.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MCEngine.Gameplay
{
    public sealed class WorldInteractionController : MonoBehaviour
    {
        private static readonly BlockType[] PlaceableBlocks =
        {
            BlockType.Dirt,
            BlockType.Stone,
            BlockType.Wood,
            BlockType.Plank,
            BlockType.StoneBrick
        };

        [SerializeField] private float interactionRange = 5f;
        [SerializeField] private float miningInterval = 0.16f;

        private Camera worldCamera;
        private GameSession session;
        private Transform player;
        private WorldTilemapView worldView;
        private float nextMiningTime;
        private int selectedBlockIndex;

        public BlockType SelectedBlock { get; private set; } = BlockType.Dirt;

        public void Initialize(
            GameSession gameSession,
            WorldTilemapView tilemapView,
            Camera interactionCamera,
            PlayerController playerController)
        {
            session = gameSession;
            worldView = tilemapView;
            worldCamera = interactionCamera;
            player = playerController.transform;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null)
            {
                return;
            }

            UpdateSelectedBlock(keyboard);
            UpdateCrafting(keyboard);

            if (keyboard.f5Key.wasPressedThisFrame)
            {
                session.Save();
            }

            if (keyboard.f9Key.wasPressedThisFrame)
            {
                session.CreateNewWorld();
            }

            if (!mouse.leftButton.isPressed)
            {
                session.CancelMining();
            }

            var shouldMine = mouse.leftButton.isPressed && Time.time >= nextMiningTime;
            var shouldPlace = mouse.rightButton.wasPressedThisFrame;
            if (!shouldMine && !shouldPlace)
            {
                return;
            }

            var cursor = worldCamera.ScreenToWorldPoint(mouse.position.ReadValue());
            var cell = worldView.WorldToCell(cursor);
            var cellCenter = (Vector2)cell + Vector2.one * 0.5f;
            if (Vector2.Distance(player.position, cellCenter) > interactionRange)
            {
                session.CancelMining();
                return;
            }

            if (shouldMine)
            {
                session.TryMine(cell);
                nextMiningTime = Time.time + miningInterval;
            }
            else
            {
                session.TryPlace(cell, SelectedBlock);
            }
        }

        private void UpdateSelectedBlock(Keyboard keyboard)
        {
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                SelectBlock(0);
            }
            else if (keyboard.digit2Key.wasPressedThisFrame)
            {
                SelectBlock(1);
            }
            else if (keyboard.digit3Key.wasPressedThisFrame)
            {
                SelectBlock(2);
            }
            else if (keyboard.digit4Key.wasPressedThisFrame)
            {
                SelectBlock(3);
            }
            else if (keyboard.digit5Key.wasPressedThisFrame)
            {
                SelectBlock(4);
            }

            var scroll = Mouse.current?.scroll.ReadValue().y ?? 0f;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                var direction = scroll > 0f ? -1 : 1;
                SelectBlock((selectedBlockIndex + direction + PlaceableBlocks.Length) % PlaceableBlocks.Length);
            }
        }

        private void UpdateCrafting(Keyboard keyboard)
        {
            if (keyboard.cKey.wasPressedThisFrame)
            {
                session.TryCraftPlanks();
            }
            else if (keyboard.bKey.wasPressedThisFrame)
            {
                session.TryCraftStoneBricks();
            }
            else if (keyboard.rKey.wasPressedThisFrame)
            {
                session.TryCraftWoodPickaxe();
            }
            else if (keyboard.tKey.wasPressedThisFrame)
            {
                session.TryCraftStonePickaxe();
            }
        }

        private void SelectBlock(int index)
        {
            selectedBlockIndex = index;
            SelectedBlock = PlaceableBlocks[index];
        }
    }
}
