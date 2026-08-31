using System.IO;
using UnityEngine;

namespace MCEngine.Persistence
{
    public sealed class SaveRepository
    {
        private const string SaveFileName = "world.json";

        public string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        public bool HasSave => File.Exists(SavePath);

        public void Save(GameSaveData saveData)
        {
            var temporaryPath = SavePath + ".tmp";
            File.WriteAllText(temporaryPath, GameSaveCodec.Serialize(saveData));

            if (File.Exists(SavePath))
            {
                File.Replace(temporaryPath, SavePath, null);
                return;
            }

            File.Move(temporaryPath, SavePath);
        }

        public GameSaveData Load()
        {
            if (!HasSave)
            {
                throw new FileNotFoundException("No saved world exists.", SavePath);
            }

            return GameSaveCodec.Deserialize(File.ReadAllText(SavePath));
        }
    }
}
