using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MCEngine.Editor
{
    [InitializeOnLoad]
    public static class InitialSceneCreator
    {
        private const string SceneDirectory = "Assets/Scenes";
        private const string ScenePath = SceneDirectory + "/Main.unity";

        static InitialSceneCreator()
        {
            EditorApplication.delayCall += EnsureMainScene;
        }

        private static void EnsureMainScene()
        {
            if (File.Exists(ScenePath) || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Directory.CreateDirectory(SceneDirectory);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            AssetDatabase.Refresh();
        }
    }
}
