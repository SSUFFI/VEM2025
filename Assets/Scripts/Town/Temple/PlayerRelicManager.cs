using System.Collections.Generic;
using UnityEngine;

public class PlayerRelicManager : MonoBehaviour
{
    public static PlayerRelicManager Inst;

    [Header("Default Relic")]
    [SerializeField] RelicDataSO defaultRelic;

    [Header("Relic Database")]
    [SerializeField]
    List<RelicDataSO> relicDatabase =
        new List<RelicDataSO>();

    public RelicDataSO equippedRelic;

    const string EQUIPPED_RELIC_KEY =
        "EquippedRelic";

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);

            LoadEquippedRelic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EquipRelic(RelicDataSO relic)
    {
        if (relic == null)
            return;

        equippedRelic = relic;

        SaveEquippedRelic();
    }

    void SaveEquippedRelic()
    {
        if (equippedRelic == null)
            return;

        if (string.IsNullOrEmpty(
            equippedRelic.relicId))
        {
            Debug.LogWarning(
                $"{equippedRelic.name}의 relicId가 없습니다.");

            return;
        }

        string key =
            AccountManager.GetAccountKey(
                EQUIPPED_RELIC_KEY);

        PlayerPrefs.SetString(
            key,
            equippedRelic.relicId);

        PlayerPrefs.Save();

        Debug.Log(
            $"장착 성유물 저장 : {equippedRelic.relicName}");
    }

    void LoadEquippedRelic()
    {
        string key =
            AccountManager.GetAccountKey(
                EQUIPPED_RELIC_KEY);

        if (!PlayerPrefs.HasKey(key))
        {
            equippedRelic = defaultRelic;

            if (equippedRelic != null)
                SaveEquippedRelic();

            return;
        }

        string savedId =
            PlayerPrefs.GetString(
                key,
                "");

        foreach (RelicDataSO relic in relicDatabase)
        {
            if (relic == null)
                continue;

            if (relic.relicId == savedId)
            {
                equippedRelic = relic;
                return;
            }
        }

        Debug.LogWarning(
            $"저장된 성유물을 찾을 수 없습니다. ID: {savedId}");

        equippedRelic = defaultRelic;

        if (equippedRelic != null)
            SaveEquippedRelic();
    }
}