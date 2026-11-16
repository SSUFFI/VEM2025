using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SlateRunState
{
    public static SlateDataSO Selected { get; private set; }
    public static int Stage { get; private set; }

    public static event Action Changed;

    public static SlateDataSO Current =>
        Stage == StageProgress.selectedStage
            ? Selected
            : null;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Selected = null;
        Stage = 0;
        Changed = null;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        if (scene.name == "Town" ||
            scene.name == "Title")
        {
            Clear();
        }
    }

    public static bool TrySelect(SlateDataSO reward)
    {
        if (reward == null || Selected != null)
            return false;

        if (reward.effectType == SlateEffectType.GrantGold)
        {
            if (reward.goldItem == null ||
                InventoryManager.Inst == null)
            {
                Debug.LogError(
                    "석판 금화 지급에 필요한 Gold 아이템 또는 " +
                    "InventoryManager가 없습니다.");

                return false;
            }
        }

        Selected = reward;
        Stage = StageProgress.selectedStage;

       
        if (reward.effectType == SlateEffectType.GrantGold)
        {
            InventoryManager.Inst.AddItem(
                reward.goldItem,
                reward.effectValue);
        }

        Changed?.Invoke();
        return true;
    }

    public static void Clear()
    {
        Selected = null;
        Stage = 0;

        Changed?.Invoke();
    }
}