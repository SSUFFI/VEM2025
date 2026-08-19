#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ItemDatabaseAutoUpdater
{
    const string ITEM_FOLDER =
        "Assets/GameData/ItemSO";

    const string DATABASE_PATH =
        "Assets/GameData/ItemSO/ItemDatabaseSO.asset";

    [MenuItem("Tools/Refresh Item Database")]
    public static void RefreshDatabase()
    {
        ItemDatabaseSO database =
            AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(
                DATABASE_PATH);

        if (database == null)
        {
            database =
                ScriptableObject.CreateInstance<ItemDatabaseSO>();

            AssetDatabase.CreateAsset(
                database,
                DATABASE_PATH);

            Debug.Log(
                "ItemDatabaseSO를 새로 생성했습니다.");
        }

        string[] guids =
            AssetDatabase.FindAssets(
                "t:ItemSO",
                new[] { ITEM_FOLDER });

        List<ItemSO> foundItems =
            new List<ItemSO>();

        HashSet<string> usedIds =
            new HashSet<string>();

        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            ItemSO item =
                AssetDatabase.LoadAssetAtPath<ItemSO>(
                    path);

            if (item == null)
                continue;

            foundItems.Add(item);

            if (string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogWarning(
                    $"Item ID가 비어있습니다 : {path}",
                    item);

                continue;
            }

            if (!usedIds.Add(item.itemId))
            {
                Debug.LogError(
                    $"중복 Item ID 발견 : {item.itemId}",
                    item);
            }
        }

        database.items = foundItems;

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Item Database 갱신 완료 : {foundItems.Count}개");
    }
}

#endif