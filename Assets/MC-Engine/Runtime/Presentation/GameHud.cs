using MCEngine.Gameplay;
using UnityEngine;

namespace MCEngine.Presentation
{
    public sealed class GameHud : MonoBehaviour
    {
        private GameSession session;
        private WorldInteractionController interaction;

        public void Initialize(GameSession gameSession, WorldInteractionController interactionController)
        {
            session = gameSession;
            interaction = interactionController;
        }

        private void OnGUI()
        {
            if (session?.Inventory == null)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(12, 12, 560, 220), GUI.skin.box);
            GUILayout.Label("MC Engine - Playable Prototype");
            GUILayout.Label("Move: A/D or arrows | Jump: Space");
            GUILayout.Label("Mine: Hold left click | Place: Right click | Range: 5 blocks");
            GUILayout.Label("Select: 1-5 or wheel | Save: F5 | New world: F9");
            GUILayout.Label("Craft: C Planks | B Stone Bricks | R Wood Pickaxe | T Stone Pickaxe");
            GUILayout.Label(
                $"Selected: {interaction.SelectedBlock} | Tool: {session.Progression.ToolTier} "
                + $"(power {session.Progression.MiningPower})");

            var inventoryText = "Inventory:";
            foreach (var slot in session.Inventory.Slots)
            {
                inventoryText += $"  {slot.BlockType} x{slot.Quantity}";
            }

            GUILayout.Label(inventoryText);
            GUILayout.Label(session.StatusMessage);
            GUILayout.EndArea();
        }
    }
}
