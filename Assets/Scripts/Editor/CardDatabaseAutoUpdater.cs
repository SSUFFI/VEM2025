#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CardDatabaseAutoUpdater
{
    const string CARD_FOLDER =
        "Assets/GameData/CardDataSO";

    const string DATABASE_PATH =
        "Assets/GameData/CardDataSO/CardDatabaseSO.asset";

    [MenuItem("Tools/Refresh Card Database")]
    public static void RefreshDatabase()
    {
        CardDatabaseSO database =
            AssetDatabase.LoadAssetAtPath<CardDatabaseSO>(
                DATABASE_PATH);

        if (database == null)
        {
            database =
                ScriptableObject.CreateInstance<CardDatabaseSO>();

            AssetDatabase.CreateAsset(
                database,
                DATABASE_PATH);

            Debug.Log(
                "CardDatabaseSO를 새로 생성했습니다.");
        }

        string[] guids =
            AssetDatabase.FindAssets(
                "t:CardDataSO",
                new[] { CARD_FOLDER });

        List<CardDataSO> foundCards =
            new List<CardDataSO>();

        HashSet<string> usedIds =
            new HashSet<string>();

        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            CardDataSO card =
                AssetDatabase.LoadAssetAtPath<CardDataSO>(
                    path);

            if (card == null)
                continue;

            foundCards.Add(card);

            if (string.IsNullOrEmpty(card.cardId))
            {
                Debug.LogWarning(
                    $"Card ID가 비어있습니다 : {path}",
                    card);

                continue;
            }

            if (!usedIds.Add(card.cardId))
            {
                Debug.LogError(
                    $"중복 Card ID 발견 : {card.cardId}",
                    card);
            }
        }

        database.cards = foundCards;

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Card Database 갱신 완료 : {foundCards.Count}장");
    }
}

#endif