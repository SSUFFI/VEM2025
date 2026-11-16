using UnityEditor;
using UnityEngine;

public static class SlateDataSetup
{
    private struct Rule
    {
        public int tier;
        public int number;
        public SlateEffectType type;
        public int value;

        public Rule(
            int tier,
            int number,
            SlateEffectType type,
            int value)
        {
            this.tier = tier;
            this.number = number;
            this.type = type;
            this.value = value;
        }

        public string Path =>
            $"Assets/GameData/SlateDataSO/Requirement{tier}/" +
            $"Slate_{tier}_{number:00}.asset";
    }

    private static readonly Rule[] Rules =
    {
        new Rule(0, 1, SlateEffectType.GrantGold, 30),
        new Rule(0, 2, SlateEffectType.ReduceFirstHeroDamage, 1),
        new Rule(0, 3, SlateEffectType.IncreaseStartingMana, 1),
        new Rule(0, 4, SlateEffectType.DrawStartingCards, 1),
        new Rule(0, 5, SlateEffectType.GrantGold, 25),
        new Rule(0, 6, SlateEffectType.IncreaseFirstAllyHealth, 1),

        new Rule(1, 1, SlateEffectType.DelayEnemyMana, 1),
        new Rule(1, 2, SlateEffectType.DrawStartingCards, 1),
        new Rule(1, 3, SlateEffectType.ReduceFirstHeroDamage, 2),
        new Rule(1, 4, SlateEffectType.IncreaseStartingMana, 1),
        new Rule(1, 5, SlateEffectType.ReduceFirstSummonCost, 1),
        new Rule(1, 6, SlateEffectType.BlockHeroDamage, 1),
        new Rule(1, 7, SlateEffectType.IncreaseStartingMana, 1),
        new Rule(1, 8, SlateEffectType.DrawStartingCards, 1),
        new Rule(1, 9, SlateEffectType.IncreaseFirstAllyAttack, 1),
        new Rule(1, 10, SlateEffectType.DelayEnemyMana, 1),
        new Rule(1, 11, SlateEffectType.IncreaseFirstAllyHealth, 1),
        new Rule(1, 12, SlateEffectType.IncreaseFirstAllyHealth, 1),

        new Rule(2, 1, SlateEffectType.BlockHeroDamage, 1),
        new Rule(2, 2, SlateEffectType.DelayEnemyMana, 2),
        new Rule(2, 3, SlateEffectType.DrawStartingCards, 2),
        new Rule(2, 4, SlateEffectType.IncreaseFirstAllyHealth, 2),
        new Rule(2, 5, SlateEffectType.IncreaseStartingMana, 2),
        new Rule(2, 6, SlateEffectType.IncreaseFirstAllyAttack, 2),
        new Rule(2, 7, SlateEffectType.DrawStartingCards, 2),
        new Rule(2, 8, SlateEffectType.IncreaseFirstAllyHealth, 2),
        new Rule(2, 9, SlateEffectType.ReduceFirstHeroDamage, 3),
        new Rule(2, 10, SlateEffectType.BlockHeroDamage, 1),
        new Rule(2, 11, SlateEffectType.IncreaseFirstAllyAttack, 2),
        new Rule(2, 12, SlateEffectType.ReduceFirstSummonCost, 2),

        new Rule(3, 1, SlateEffectType.DrawStartingCards, 2),
        new Rule(3, 2, SlateEffectType.IncreaseStartingMana, 2),
        new Rule(3, 3, SlateEffectType.IncreaseFirstAllyHealth, 3),
        new Rule(3, 4, SlateEffectType.DrawStartingCards, 2),
        new Rule(3, 5, SlateEffectType.BlockHeroDamage, 2),
        new Rule(3, 6, SlateEffectType.ReduceFirstSummonCost, 2),
        new Rule(3, 7, SlateEffectType.IncreaseFirstAllyAttack, 3),
        new Rule(3, 8, SlateEffectType.BlockHeroDamage, 2),
        new Rule(3, 9, SlateEffectType.DelayEnemyMana, 3),
        new Rule(3, 10, SlateEffectType.IncreaseFirstAllyHealth, 3),
        new Rule(3, 11, SlateEffectType.GrantGold, 100),
        new Rule(3, 12, SlateEffectType.DelayEnemyMana, 3)
    };

    [MenuItem("VEM/Slate/Apply Existing 42 Effects")]
    public static void Apply()
    {
        if (Application.isPlaying)
        {
            Debug.LogError(
                "Play Mode를 종료한 뒤 실행하세요.");

            return;
        }

        ItemSO gold = AssetDatabase.LoadAssetAtPath<ItemSO>(
            "Assets/GameData/ItemDataSO/Gold.asset");

        if (gold == null)
        {
            Debug.LogError("Gold.asset을 찾을 수 없습니다.");
            return;
        }

        SlateDataSO[] rewards =
            new SlateDataSO[Rules.Length];

        // 모든 파일을 찾은 뒤에만 실제 수정을 시작합니다.
        for (int i = 0; i < Rules.Length; i++)
        {
            rewards[i] =
                AssetDatabase.LoadAssetAtPath<SlateDataSO>(
                    Rules[i].Path);

            if (rewards[i] == null)
            {
                Debug.LogError(
                    "석판을 찾을 수 없습니다: " +
                    Rules[i].Path);

                return;
            }
        }

        for (int i = 0; i < Rules.Length; i++)
        {
            Rule rule = Rules[i];
            SlateDataSO reward = rewards[i];

            Undo.RecordObject(
                reward,
                "Set Slate Effect");

            reward.effectType = rule.type;
            reward.effectValue = rule.value;

            // 길손의 몫만 룬을 표시하지 않습니다.
            reward.showRune =
                !(rule.tier == 0 && rule.number == 1);

            reward.goldItem =
                rule.type == SlateEffectType.GrantGold
                    ? gold
                    : null;

            EditorUtility.SetDirty(reward);
        }

        AssetDatabase.SaveAssets();

        Debug.Log(
            "석판 42개 효과 설정 완료. " +
            "각 석판의 Rune Icon을 지정하세요.");
    }
}
