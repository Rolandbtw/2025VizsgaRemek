using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ReloadScripts
{
    static ReloadScripts()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Exiting Play Mode... Reloading Script.");
            AssetDatabase.ImportAsset("Assets/Scripts/Editor/InventoryTest.cs", ImportAssetOptions.DontDownloadFromCacheServer);
        }
    }
}