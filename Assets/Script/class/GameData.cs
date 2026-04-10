using System.IO;
using UnityEngine;

public class GameData
{
    public static int highScore;
    public static int currentScore;
    public static int sceneLevel = 1; // level đang chơi (để load đúng level map)

    public static ShipData shipData;
    public static LevelData levelData;

    static string path = Application.persistentDataPath + "/save.json";

    [System.Serializable]
    class SaveData
    {
        public int highScore;
        public string shipName;
        public int sceneLevel;
    }

    // LOAD
    public static void Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            sceneLevel = data.sceneLevel;

            if (sceneLevel < 1) sceneLevel = 1; // đảm bảo level luôn >=1
            Debug.Log($"✓ Load thành công! HighScore: {highScore}, Level: {sceneLevel}");

            // 👉 thử load ship đã lưu
            if (!string.IsNullOrEmpty(data.shipName))
            {
                shipData = Resources.Load<ShipData>("Ships/" + data.shipName);
            }
        }
        else
        {
            Debug.LogWarning($"File save không tồn tại: {path}");
            sceneLevel = 1; // default level
        }

        // 👉 fallback nếu null
        if (shipData == null)
        {
            Debug.LogWarning("Load ship fail → dùng Ship_Yellow");
            shipData = Resources.Load<ShipData>("Ships/Ship_Yellow");
        }
    }

    // SAVE
    public static void Save()
    {
        try
        {
            SaveData data = new SaveData();

            data.highScore = highScore;
            data.shipName = shipData != null ? shipData.name : "Ship_Yellow";
            data.sceneLevel = sceneLevel;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            // C:/Users/Van Bang/AppData/LocalLow/DefaultCompany/spacecraft/save.json
            
            Debug.Log($"✓ Save thành công! Path: {path}\nHighScore: {highScore}\nShip: {data.shipName}\nLevel: {data.sceneLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Lỗi save: {e.Message}");
        }
    }
}