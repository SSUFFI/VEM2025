public enum BattleMode
{
    Normal,
    Tutorial1,
    Tutorial2,
    Tutorial3,
    Mock
}

public static class BattleData
{
    public static DeckSO selectedEnemyDeck;
    public static DeckSO tutorialEnemyDeck;

    public static bool isTutorialBattle = false;

    public static BattleMode battleMode =
        BattleMode.Normal;

    public static NodeType selectedNodeType;

    public static bool IsBattleTutorial
    {
        get
        {
            return battleMode == BattleMode.Tutorial1 ||
                   battleMode == BattleMode.Tutorial2 ||
                   battleMode == BattleMode.Tutorial3;
        }
    }

    public static void SetBattleMode(
        BattleMode mode)
    {
        battleMode = mode;

        isTutorialBattle = IsBattleTutorial;
    }

    public static void ResetBattleMode()
    {
        battleMode = BattleMode.Normal;
        isTutorialBattle = false;
        tutorialEnemyDeck = null;
    }
}