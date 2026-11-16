using System;

public sealed class SlateBattleEffectState
{
    private readonly SlateEffectType type;
    private readonly int value;

    private int enemyDelayRemaining;
    private int blocksRemaining;

    private bool usedFirstTurn;
    private bool usedFirstDamage;
    private bool usedFirstSummon;
    private bool usedFirstCard;
    private bool usedDraw;

    public SlateBattleEffectState(
        SlateEffectType type,
        int value)
    {
        this.type = type;
        this.value = Math.Max(0, value);

        enemyDelayRemaining =
            type == SlateEffectType.DelayEnemyMana
                ? this.value
                : 0;

        blocksRemaining =
            type == SlateEffectType.BlockHeroDamage
                ? this.value
                : 0;
    }

    public int ConsumeStartingDraws()
    {
        if (usedDraw ||
            type != SlateEffectType.DrawStartingCards)
        {
            return 0;
        }

        usedDraw = true;
        return value;
    }

    public int ConsumeManaIncrease(bool isMine)
    {
        // 플레이어 턴은 보스 마나 지연 횟수를 소모하지 않습니다.
        if (!isMine && enemyDelayRemaining > 0)
        {
            enemyDelayRemaining--;
            return 0;
        }

        return 1;
    }

    public int ConsumeFirstTurnMana(bool isMine)
    {
        if (!isMine || usedFirstTurn)
            return 0;

        usedFirstTurn = true;

        return type == SlateEffectType.IncreaseStartingMana
            ? value
            : 0;
    }

    public int ResolveHeroDamage(int damage)
    {
        // 0 피해로는 방어 횟수를 소모하지 않습니다.
        if (damage <= 0)
            return 0;

        if (blocksRemaining > 0)
        {
            blocksRemaining--;
            return 0;
        }

        if (!usedFirstDamage &&
            type == SlateEffectType.ReduceFirstHeroDamage)
        {
            usedFirstDamage = true;
            return Math.Max(0, damage - value);
        }

        return damage;
    }

    public int GetSummonCost(int baseCost)
    {
        int discount =
            !usedFirstCard &&
            type == SlateEffectType.ReduceFirstSummonCost
                ? value
                : 0;

        return Math.Max(0, baseCost - discount);
    }

    // 실제 카드 소환에 성공했을 때만 호출합니다.
    public void OnSummonCardPlayed()
    {
        usedFirstCard = true;
    }

    public void ConsumeFirstAllyBonus(
        out int attack,
        out int health)
    {
        attack = 0;
        health = 0;

        if (usedFirstSummon)
            return;

        usedFirstSummon = true;

        if (type == SlateEffectType.IncreaseFirstAllyAttack)
            attack = value;

        if (type == SlateEffectType.IncreaseFirstAllyHealth)
            health = value;
    }
}