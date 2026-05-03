using UnityEngine;

/// <summary>
/// Handles saving and loading of GameData using PlayerPrefs (JSON).
/// Architecture is designed to easily swap PlayerPrefs for a Cloud provider later.
/// </summary>
public static class SaveManager
{
    private const string SAVE_KEY = "bachlism_save_data_v1";

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        // TODO: Cloud Integration
        // e.g., CloudSaveProvider.Save(json);
    }

    public static GameData Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            return JsonUtility.FromJson<GameData>(json);
        }
        
        // TODO: Cloud Integration
        // e.g., string cloudJson = CloudSaveProvider.Load();
        // if (!string.IsNullOrEmpty(cloudJson)) return JsonUtility.FromJson<GameData>(cloudJson);

        return null;
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        // TODO: Cloud Integration -> CloudSaveProvider.Delete();
    }
}