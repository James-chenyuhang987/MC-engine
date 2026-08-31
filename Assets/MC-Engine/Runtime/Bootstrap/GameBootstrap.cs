using MCEngine.Configuration;
using MCEngine.Gameplay;
using MCEngine.Player;
using MCEngine.Presentation;
using UnityEngine;

namespace MCEngine.Bootstrap
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartGame()
        {
            if (Object.FindAnyObjectByType<GameSession>() != null)
            {
                return;
            }

            var contentCatalog = Resources.Load<GameContentCatalog>("GameContentCatalog");
            var root = new GameObject("MC Engine");
            var session = root.AddComponent<GameSession>();
            var worldView = WorldTilemapView.Create(root.transform, contentCatalog);
            var player = CreatePlayer(root.transform, contentCatalog);
            var worldCamera = CreateCamera(root.transform, player.transform);
            var interaction = root.AddComponent<WorldInteractionController>();
            var hud = root.AddComponent<GameHud>();

            session.Initialize(worldView, player.transform);
            interaction.Initialize(session, worldView, worldCamera, player);
            hud.Initialize(session, interaction);
        }

        private static PlayerController CreatePlayer(Transform parent, GameContentCatalog contentCatalog)
        {
            var playerObject = new GameObject(
                "Player",
                typeof(SpriteRenderer),
                typeof(Rigidbody2D),
                typeof(BoxCollider2D),
                typeof(PlayerController));
            playerObject.transform.SetParent(parent);

            var renderer = playerObject.GetComponent<SpriteRenderer>();
            renderer.sprite = contentCatalog?.PlayerSprite
                              ?? RuntimeSpriteFactory.CreateSolidSprite(
                                  "Player",
                                  new Color(0.23f, 0.67f, 0.95f));
            renderer.sortingOrder = 10;
            var spriteSize = renderer.sprite.bounds.size;
            playerObject.transform.localScale = new Vector3(
                0.8f / spriteSize.x,
                1.8f / spriteSize.y,
                1f);

            var body = playerObject.GetComponent<Rigidbody2D>();
            body.freezeRotation = true;
            body.gravityScale = 2.5f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = playerObject.GetComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            return playerObject.GetComponent<PlayerController>();
        }

        private static Camera CreateCamera(Transform parent, Transform player)
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(CameraFollow));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(parent);

            var worldCamera = cameraObject.GetComponent<Camera>();
            worldCamera.orthographic = true;
            worldCamera.orthographicSize = 11f;
            worldCamera.backgroundColor = new Color(0.39f, 0.67f, 0.92f);
            worldCamera.clearFlags = CameraClearFlags.SolidColor;

            cameraObject.GetComponent<CameraFollow>().Initialize(player);
            return worldCamera;
        }
    }
}
