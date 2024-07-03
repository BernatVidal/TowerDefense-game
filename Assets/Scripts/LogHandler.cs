using UnityEngine;

/// <summary>
/// Class used in Debug purposes during the development. Have the hability of closing the game if required
/// </summary>
public static class LogHandler
{
    static string errorColor = "<color=red>";
    static string warningColor = "<color=yellow>";
    static string infoColor = "<color=cyan>";
    static string colorEnd = "</color>";

    public static void LogInfo(string message)
    {
        Debug.Log($"{infoColor} {message} {colorEnd}");
    }

    public static void LogWarning(string message)
    {
        Debug.Log($"{warningColor} {message} {colorEnd}");
    }

    public static void LogError(string message, bool closeApp = false)
    {
        Debug.Log($"{errorColor} {message} {colorEnd}");
        if (closeApp)
            CloseApp();
    }

    static void CloseApp()
    {
        Debug.LogWarning("Quitting application due to the mentioned error above.");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
