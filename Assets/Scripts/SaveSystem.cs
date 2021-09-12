using UnityEngine;
using System.IO;

namespace ZombieFight
{
    public class SaveSystem
    {
        private readonly string filePath;
        public SaveSystem()
        {
            filePath = Application.persistentDataPath + "/ZombieFight.save";
        }

        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
        }

        public SaveData Load()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveData data = new SaveData();
                try
                {
                    data = JsonUtility.FromJson<SaveData>(json);
                }
                catch
                {
                    Debug.LogWarning("SaveSystem:Load() – SaveFile was malformed.\n" + json);
                }
                return data;
            }
            else
            {
                SaveData data = new SaveData();
                data.hiScore = 0;
                data.maxLevel = 0;
                return data;
            }
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int hiScore;
        public int maxLevel;
    }

}