using UnityEngine;

public static class PlayerPrefsX
{
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key, 0) != 0;
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
    }

    public static void SetColor(string key, Color32 value)
    {
        PlayerPrefs.SetInt(key, value.PackedColor);
    }

    public static Color32 GetColor(string key)
    {
        return new Color32(PlayerPrefs.GetInt(key, 0));
    }
    
    public static Color32 GetColor(string key, int defaultValue)
    {
        return new Color32(PlayerPrefs.GetInt(key, defaultValue));
    }
    
    public static Color32 GetColor(string key, Color32 defaultValue)
    {
        return new Color32(PlayerPrefs.GetInt(key, defaultValue.PackedColor));
    }
}
