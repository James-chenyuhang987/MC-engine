using System;
using UnityEngine;

namespace MCEngine.Persistence
{
    public static class GameSaveCodec
    {
        public static string Serialize(GameSaveData saveData)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            saveData.Validate();
            return JsonUtility.ToJson(saveData, true);
        }

        public static GameSaveData Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Save data cannot be empty.", nameof(json));
            }

            var saveData = JsonUtility.FromJson<GameSaveData>(json);
            if (saveData == null)
            {
                throw new InvalidOperationException("Save data could not be deserialized.");
            }

            saveData.Validate();
            return saveData;
        }
    }
}
