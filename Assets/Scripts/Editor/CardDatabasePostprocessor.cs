#if UNITY_EDITOR

using UnityEditor;

public class CardDatabasePostprocessor :
    AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        bool needsRefresh = false;

        foreach (string path in importedAssets)
        {
            if (path.StartsWith(
                    "Assets/GameData/CardDataSO"))
            {
                needsRefresh = true;
                break;
            }
        }

        if (!needsRefresh)
        {
            foreach (string path in deletedAssets)
            {
                if (path.StartsWith(
                        "Assets/GameData/CardDataSO"))
                {
                    needsRefresh = true;
                    break;
                }
            }
        }

        if (!needsRefresh)
        {
            foreach (string path in movedAssets)
            {
                if (path.StartsWith(
                        "Assets/GameData/CardDataSO"))
                {
                    needsRefresh = true;
                    break;
                }
            }
        }

        if (!needsRefresh)
            return;

        EditorApplication.delayCall += () =>
        {
            CardDatabaseAutoUpdater.RefreshDatabase();
        };
    }
}

#endif