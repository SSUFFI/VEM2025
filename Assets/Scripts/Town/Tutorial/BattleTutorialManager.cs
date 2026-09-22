using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Tutorial1Step
{
    None,
    PlayFirstCard,
    FirstCardPlayed,
    EnemyTurn1,
    AttackTauntDummy,
    FirstAttackComplete,
    EnemyTurn2,
    AttackEnemyHero,
    Complete
}

public enum Tutorial2Step
{
    None,
    PlayTauntCard,
    TauntCardPlayed,
    EnemyTurn1,
    EnemyTurn1Complete,
    UseRelic,
    RelicUsed,
    AttackBigEnemy,
    Complete
}

public enum Tutorial3Step
{
    None,

    AttackEnemyDeck,

    EnemyJudgmentTriggered,

    OpenEnemyGrave,
    PreviewEnemyJudgment,
    EnemyJudgmentChecked,

    EndTurn,

    EnemyTurn,

    OpenMyGrave,
    PreviewMyJudgment,

    Complete
}


public class BattleTutorialManager : MonoBehaviour
{
    public static BattleTutorialManager Inst;

    public bool IsActive =>
        BattleData.IsBattleTutorial;

    [Header("Debug")]
    [SerializeField] bool debugTutorial1;
    [SerializeField] bool debugTutorial2;
    [SerializeField] bool debugTutorial3;

    [Header("Tutorial 1 Cards")]
    [SerializeField] CardDataSO tutorial1PlayerCard;
    [SerializeField] CardDataSO tutorial1TauntDummy;
    [SerializeField] CardDataSO tutorial1EnemyCard;

    [Header("Tutorial 1 State")]
    [SerializeField]
    Tutorial1Step tutorial1Step = Tutorial1Step.None;

    [Header("Tutorial 2 Cards")]
    [SerializeField] CardDataSO tutorial2TauntCard;
    [SerializeField] CardDataSO tutorial2EnemySmallCard;
    [SerializeField] CardDataSO tutorial2EnemyBigCard;

    [Header("Tutorial 2 State")]
    [SerializeField]
    Tutorial2Step tutorial2Step = Tutorial2Step.None;

    [Header("Tutorial 2 Relic")]
    [SerializeField] RelicDataSO tutorial2Relic;

    public RelicDataSO Tutorial2Relic => tutorial2Relic;

    [Header("Tutorial 3 Cards")]
    [SerializeField] CardDataSO tutorial3PlayerCard;
    [SerializeField] CardDataSO tutorial3EnemyJudgmentCard;
    [SerializeField] CardDataSO tutorial3WolfCard;
    [SerializeField] CardDataSO tutorial3PlayerJudgmentCard;

    [Header("Tutorial 3 State")]
    [SerializeField]
    Tutorial3Step tutorial3Step = Tutorial3Step.None;

    int tutorial3EnemyJudgmentGraveCount;

    void Awake()
    {
        Inst = this;

#if UNITY_EDITOR
    if (debugTutorial1)
    {
        BattleData.SetBattleMode(BattleMode.Tutorial1);
    }
    else if (debugTutorial2)
    {
        BattleData.SetBattleMode(BattleMode.Tutorial2);
    }
    else if (debugTutorial3)
    {
        BattleData.SetBattleMode(BattleMode.Tutorial3);
    }
#endif
    }

    public void StartTutorial()
    {
        if (!IsActive)
            return;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:
                StartTutorial1();
                break;

            case BattleMode.Tutorial2:
                StartTutorial2();
                break;

            case BattleMode.Tutorial3:
                StartTutorial3();
                break;
        }
    }

    void StartTutorial1()
    {
        if (tutorial1PlayerCard == null || tutorial1TauntDummy == null || tutorial1EnemyCard == null)
        {
            return;
        }

        if (CardManager.Inst == null || TurnManager.Inst == null)
        {
            return;
        }

        CardManager.Inst.SetupDecks();

        Card tutorialCard = CardManager.Inst.AddTutorialCardToHand(tutorial1PlayerCard, true);

        if (BattleTutorialHighlight.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimAll();

            if (tutorialCard != null)
            {
                BattleTutorialHighlight.Inst.Highlight(tutorialCard.gameObject);
            }

            if (BattleTutorialUI.Inst != null && BattleTutorialUI.Inst.GuidePanel != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleTutorialUI.Inst.GuidePanel);
            }

            if (EntityManager.Inst != null)
            {
                BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetPicker);

                BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetArrow);
            }
        }

        tutorial1Step = Tutorial1Step.PlayFirstCard;

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("카드를 드래그해 필드에 하수인으로 배치하세요.");
        }

        StartCoroutine(TurnManager.Inst.StartTutorialGameCo());
    }

    public void SetupTutorialDecks(out List<CardDataSO> myDeck, out List<CardDataSO> enemyDeck)
    {
        myDeck = new List<CardDataSO>();
        enemyDeck = new List<CardDataSO>();

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:
                SetupTutorial1Decks(myDeck, enemyDeck);
                break;

            case BattleMode.Tutorial2:
                SetupTutorial2Decks(myDeck, enemyDeck);
                break;

            case BattleMode.Tutorial3:
                SetupTutorial3Decks(myDeck, enemyDeck);
                break;
        }
    }

    void SetupTutorial1Decks(List<CardDataSO> myDeck, List<CardDataSO> enemyDeck)
    {
        myDeck.Add(tutorial1PlayerCard);
        enemyDeck.Add(tutorial1EnemyCard);
    }

    void SetupTutorial2Decks(List<CardDataSO> myDeck, List<CardDataSO> enemyDeck)
    {
    }

    void SetupTutorial3Decks(List<CardDataSO> myDeck, List<CardDataSO> enemyDeck)
    {
        for (int i = 0; i < 10; i++)
        {
            enemyDeck.Add(tutorial3EnemyJudgmentCard);
        }

        for (int i = 0; i < 10; i++)
        {
            myDeck.Add(tutorial3PlayerJudgmentCard);
        }
    }

    public void OnCardPlayed(CardDataSO cardData, bool isMine)
    {
        if (!IsActive)
            return;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:

                if (tutorial1Step != Tutorial1Step.PlayFirstCard)
                    return;

                if (!isMine)
                    return;

                if (cardData != tutorial1PlayerCard)
                    return;

                tutorial1Step = Tutorial1Step.FirstCardPlayed;

                if (BattleTutorialHighlight.Inst != null && EndTurnBtn.Inst != null)
                {
                    BattleTutorialHighlight.Inst.Highlight(EndTurnBtn.Inst.gameObject);
                }

                if (BattleTutorialUI.Inst != null)
                {
                    BattleTutorialUI.Inst.ShowGuide("하수인은 배치된 다음 턴부터 공격할 수 있습니다.\n\n턴 종료 버튼을 누르세요.");
                }

                break;


            case BattleMode.Tutorial2:

                if (tutorial2Step != Tutorial2Step.PlayTauntCard)
                    return;

                if (!isMine)
                    return;

                if (cardData != tutorial2TauntCard)
                    return;

                tutorial2Step =
                    Tutorial2Step.TauntCardPlayed;

                if (BattleTutorialHighlight.Inst != null && EndTurnBtn.Inst != null)
                {
                    BattleTutorialHighlight.Inst.Highlight(EndTurnBtn.Inst.gameObject);
                }

                if (BattleTutorialUI.Inst != null)
                {
                    BattleTutorialUI.Inst.ShowGuide("도발을 가진 하수인이 있으면\n적은 그 하수인을 먼저 공격해야 합니다.\n\n턴 종료 버튼을 누르세요.");
                }

                break;
        }
    }

    public bool CanEndTurn()
    {
        if (!IsActive)
            return true;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:
                return tutorial1Step == Tutorial1Step.FirstCardPlayed || tutorial1Step == Tutorial1Step.FirstAttackComplete;

            case BattleMode.Tutorial2:
                return tutorial2Step == Tutorial2Step.TauntCardPlayed;
        }

        return false;
    }

    public void OnTutorialEnemyTurnStarted()
    {
        if (!IsActive)
            return;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:

                if (tutorial1Step == Tutorial1Step.FirstCardPlayed)
                {
                    StartCoroutine(Tutorial1EnemyTurn1Co());

                    return;
                }

                if (tutorial1Step == Tutorial1Step.FirstAttackComplete)
                {
                    StartCoroutine(Tutorial1EnemyTurn2Co());

                    return;
                }

                break;

            case BattleMode.Tutorial2:

                if (tutorial2Step ==
                    Tutorial2Step.TauntCardPlayed)
                {
                    StartCoroutine(
                        Tutorial2EnemyTurn1Co());

                    return;
                }

                break;
        }
    }

    IEnumerator Tutorial1EnemyTurn1Co()
    {
        tutorial1Step =
            Tutorial1Step.EnemyTurn1;

        if (BattleTutorialHighlight.Inst != null && EndTurnBtn.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimTarget(EndTurnBtn.Inst.gameObject);
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.HideGuide();
        }

        yield return new WaitForSeconds(0.7f);

        bool success = EntityManager.Inst.SpawnEntity(false, tutorial1TauntDummy, EntityManager.Inst.OtherDeckSpawnPos);

        if (!success)
            yield break;

        yield return new WaitForSeconds(1.2f);

        tutorial1Step = Tutorial1Step.AttackTauntDummy;

        TurnManager.Inst.EndTurn();

        if (BattleTutorialHighlight.Inst != null)
        {
            Entity playerEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && x.Data == tutorial1PlayerCard);

            Entity enemyEntity = EntityManager.Inst.OtherEntities.Find(x => x != null && !x.isBossOrEmpty && x.Data == tutorial1TauntDummy);

            if (playerEntity != null)
            {
                BattleTutorialHighlight.Inst.Highlight(playerEntity.gameObject);
            }

            if (enemyEntity != null)
            {
                BattleTutorialHighlight.Inst.Highlight(enemyEntity.gameObject);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("내 하수인을 드래그하여 공격 목표를 정할 수 있습니다.\n\n적 하수인을 공격하세요.");
        }
    }

    IEnumerator Tutorial1EnemyTurn2Co()
    {
        tutorial1Step = Tutorial1Step.EnemyTurn2;

        if (BattleTutorialHighlight.Inst != null && EndTurnBtn.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimTarget(EndTurnBtn.Inst.gameObject);
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.HideGuide();
        }

        yield return new WaitForSeconds(0.7f);

        bool success = EntityManager.Inst.SpawnEntity(false, tutorial1EnemyCard, EntityManager.Inst.OtherDeckSpawnPos);

        if (!success)
            yield break;

        yield return new WaitForSeconds(1.2f);

        tutorial1Step = Tutorial1Step.AttackEnemyHero;

        TurnManager.Inst.EndTurn();

        if (BattleTutorialHighlight.Inst != null)
        {
            Entity playerEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && x.Data == tutorial1PlayerCard);

            if (playerEntity != null)
            {
                BattleTutorialHighlight.Inst.Highlight(playerEntity.gameObject);
            }

            Entity enemyBoss = EntityManager.Inst.OtherBossEntity;

            if (enemyBoss != null)
            {
                BattleTutorialHighlight.Inst.Highlight(enemyBoss.gameObject);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("적의 체력이 얼마 남지 않았습니다.\n\n적 하수인을 무시하고 적을 직접 공격하세요.");
        }
    }

    IEnumerator Tutorial2EnemyTurn1Co()
    {
        tutorial2Step = Tutorial2Step.EnemyTurn1;

        if (BattleTutorialHighlight.Inst != null && EndTurnBtn.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimTarget(EndTurnBtn.Inst.gameObject);
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.HideGuide();
        }

        yield return new WaitForSeconds(0.7f);

        bool success = EntityManager.Inst.SpawnEntity(false, tutorial2EnemyBigCard, EntityManager.Inst.OtherDeckSpawnPos);

        if (!success)
            yield break;

        yield return new WaitForSeconds(1.2f);

        Entity tauntEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && !x.isDie && x.Data == tutorial2TauntCard);

        if (tauntEntity == null)
            yield break;

        List<Entity> attackers = EntityManager.Inst.OtherEntities.FindAll(x => x != null && !x.isBossOrEmpty && !x.isDie && x.Data == tutorial2EnemySmallCard);

        foreach (Entity attacker in attackers)
        {
            if (attacker == null ||attacker.isDie || tauntEntity == null || tauntEntity.isDie)
            {
                continue;
            }

            EntityManager.Inst.TutorialAttack(attacker, tauntEntity);

            yield return new WaitForSeconds(1.7f);
        }

        tutorial2Step = Tutorial2Step.EnemyTurn1Complete;

        yield return new WaitForSeconds(0.5f);

        tutorial2Step = Tutorial2Step.UseRelic;

        TurnManager.Inst.EndTurn();

        yield return new WaitForSeconds(0.8f);

        if (BattleTutorialHighlight.Inst != null)
        {
            if (tauntEntity != null)
            {
                BattleTutorialHighlight.Inst.Highlight(tauntEntity.gameObject);
            }

            if (BattleRelicUI.Inst != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleRelicUI.Inst.gameObject);

                BattleTutorialHighlight.Inst.Highlight(BattleRelicUI.Inst.ManaCostRoot);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("성유물은 마나를 소모하여 사용할 수 있습니다.\n\n성유물을 사용해 도발 하수인의 체력을 회복하세요.");
        }
    }

    public bool CanAttack(Entity attacker, Entity defender)
    {
        if (!IsActive)
            return true;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:

                if (attacker == null || defender == null)
                    return false;

                if (!attacker.isMine)
                    return false;

                if (attacker.Data != tutorial1PlayerCard)
                    return false;

                if (tutorial1Step == Tutorial1Step.AttackTauntDummy)
                {
                    if (defender.isMine)
                        return false;

                    if (defender.isBossOrEmpty)
                        return false;

                    if (defender.Data != tutorial1TauntDummy)
                        return false;

                    return true;
                }

                if (tutorial1Step == Tutorial1Step.AttackEnemyHero)
                {
                    if (defender.isMine)
                        return false;

                    if (!defender.isBossOrEmpty)
                        return false;

                    return true;
                }

                return false;


            case BattleMode.Tutorial2:

                if (tutorial2Step != Tutorial2Step.AttackBigEnemy)
                    return false;

                if (attacker == null || defender == null)
                    return false;

                if (!attacker.isMine)
                    return false;

                if (attacker.Data != tutorial2TauntCard)
                    return false;

                if (defender.isMine || defender.isBossOrEmpty)
                    return false;

                if (defender.Data != tutorial2EnemyBigCard)
                    return false;

                return true;


            case BattleMode.Tutorial3:

                if (tutorial3Step != Tutorial3Step.AttackEnemyDeck)
                    return false;

                if (attacker == null || defender == null)
                    return false;

                if (!attacker.isMine)
                    return false;

                if (attacker.Data != tutorial3PlayerCard)
                    return false;

                if (defender.isMine)
                    return false;

                if (!defender.isBossOrEmpty)
                    return false;

                return true;
        }

        return false;
    }

    public void OnAttackResolved(Entity attacker, Entity defender)
    {
        if (!IsActive)
            return;

        if (attacker == null || defender == null)
            return;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:

                if (tutorial1Step != Tutorial1Step.AttackTauntDummy)
                    return;

                if (attacker.Data != tutorial1PlayerCard)
                    return;

                if (defender.Data != tutorial1TauntDummy)
                    return;

                tutorial1Step = Tutorial1Step.FirstAttackComplete;

                if (BattleTutorialHighlight.Inst != null)
                {
                    BattleTutorialHighlight.Inst.DimTarget(attacker.gameObject);

                    BattleTutorialHighlight.Inst.DimTarget(defender.gameObject);

                    if (EndTurnBtn.Inst != null)
                    {
                        BattleTutorialHighlight.Inst.Highlight(EndTurnBtn.Inst.gameObject);
                    }
                }

                if (BattleTutorialUI.Inst != null)
                {
                    BattleTutorialUI.Inst.ShowGuide("하수인은 서로의 공격력만큼 피해를 주고받습니다.\n\n턴 종료 버튼을 누르세요.");
                }

                break;

            case BattleMode.Tutorial2:

                if (tutorial2Step != Tutorial2Step.AttackBigEnemy)
                    return;

                if (attacker.Data != tutorial2TauntCard)
                    return;

                if (defender.Data != tutorial2EnemyBigCard)
                    return;

                CompleteTutorial2(attacker, defender);
                break;
        }
    }

    public void OnTutorialWin()
    {
        if (!IsActive)
            return;

        switch (BattleData.battleMode)
        {
            case BattleMode.Tutorial1:

                if (tutorial1Step != Tutorial1Step.AttackEnemyHero)
                    return;

                CompleteTutorial1();
                break;
        }
    }

    void CompleteTutorial1()
    {
        tutorial1Step = Tutorial1Step.Complete;

        if (GameResultManager.Inst != null)
            GameResultManager.Inst.isGameOver = true;

        if (BattleTutorialHighlight.Inst != null && EntityManager.Inst != null)
        {
            Entity playerEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && x.Data == tutorial1PlayerCard);

            if (playerEntity != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(playerEntity.gameObject);
            }

            Entity enemyBoss = EntityManager.Inst.OtherBossEntity;

            if (enemyBoss != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(enemyBoss.gameObject);
            }

            if (BattleTutorialUI.Inst != null && BattleTutorialUI.Inst.GuidePanel != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleTutorialUI.Inst.GuidePanel);
            }
        }

        StartCoroutine(CompleteTutorial1Co());
    }

    IEnumerator CompleteTutorial1Co()
    {
        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("적의 덱이 모두 소진되었습니다.\n\n훈련을 종료합니다.");
        }

        yield return new WaitForSeconds(2f);

        TrainingSelectManager.CompleteTraining1();
        TrainingSelectManager.OpenPanelAfterReturn();

        BattleData.ResetBattleMode();

        SceneManager.LoadScene("Town");
    }

    void StartTutorial2()
    {
        if (tutorial2TauntCard == null || tutorial2EnemySmallCard == null || tutorial2EnemyBigCard == null)
        {
            return;
        }

        if (CardManager.Inst == null || TurnManager.Inst == null)
        {
            return;
        }

        CardManager.Inst.SetupDecks();

        for (int i = 0; i < 4; i++)
        {
            EntityManager.Inst.SpawnEntity(false, tutorial2EnemySmallCard, EntityManager.Inst.OtherDeckSpawnPos);
        }

        tutorial2Step = Tutorial2Step.PlayTauntCard;

        Card tutorialCard = CardManager.Inst.AddTutorialCardToHand(tutorial2TauntCard, true);

        if (BattleTutorialHighlight.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimAll();

            if (tutorialCard != null)
            {
                BattleTutorialHighlight.Inst.Highlight(tutorialCard.gameObject);
            }

            if (BattleTutorialUI.Inst != null &&
                BattleTutorialUI.Inst.GuidePanel != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleTutorialUI.Inst.GuidePanel);
            }

            if (EntityManager.Inst != null)
            {
                BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetPicker);

                BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetArrow);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("도발 능력을 가진 하수인을 필드에 배치하세요.");
        }

        StartCoroutine(TurnManager.Inst.StartTutorialGameCo());
    }

    public void OnRelicUsed(RelicDataSO relic, Entity target)
    {
        if (!IsActive)
            return;

        if (BattleData.battleMode != BattleMode.Tutorial2)
            return;

        if (tutorial2Step != Tutorial2Step.UseRelic)
            return;

        if (relic != tutorial2Relic)
            return;

        if (target == null || target.isDie || target.Data != tutorial2TauntCard)
        {
            return;
        }

        tutorial2Step = Tutorial2Step.RelicUsed;

        if (BattleTutorialHighlight.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimTarget(BattleRelicUI.Inst.gameObject);

            BattleTutorialHighlight.Inst.DimTarget(BattleRelicUI.Inst.ManaCostRoot);

            BattleTutorialHighlight.Inst.DimTarget(target.gameObject);
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("성유물은 한 턴에 한 번만 사용할 수 있습니다.");
        }
        StartCoroutine(Tutorial2AfterRelicCo());
    }

    IEnumerator Tutorial2AfterRelicCo()
    {
        yield return new WaitForSeconds(1.5f);

        Entity tauntEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && !x.isDie && x.Data == tutorial2TauntCard);

        Entity bigEnemy = EntityManager.Inst.OtherEntities.Find(x => x != null && !x.isBossOrEmpty && !x.isDie && x.Data == tutorial2EnemyBigCard);

        if (tauntEntity == null || bigEnemy == null)
        {
            yield break;
        }

        tutorial2Step = Tutorial2Step.AttackBigEnemy;

        if (BattleTutorialHighlight.Inst != null)
        {
            BattleTutorialHighlight.Inst.Highlight(tauntEntity.gameObject);

            BattleTutorialHighlight.Inst.Highlight(bigEnemy.gameObject);
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("회복한 하수인으로 적 하수인을 공격하세요.");
        }
    }

    void CompleteTutorial2(Entity attacker, Entity defender)
    {
        tutorial2Step = Tutorial2Step.Complete;

        if (GameResultManager.Inst != null)
            GameResultManager.Inst.isGameOver = true;

        if (BattleTutorialHighlight.Inst != null)
        {
            if (attacker != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(attacker.gameObject);
            }

            if (defender != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(defender.gameObject);
            }

            if (BattleTutorialUI.Inst != null && BattleTutorialUI.Inst.GuidePanel != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleTutorialUI.Inst.GuidePanel);
            }
        }

        StartCoroutine(CompleteTutorial2Co());
    }

    IEnumerator CompleteTutorial2Co()
    {
        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("성유물과 하수인의 능력을 활용해 적을 처치했습니다.\n\n훈련을 종료합니다.");
        }

        yield return new WaitForSeconds(2f);

        TrainingSelectManager.CompleteTraining2();
        TrainingSelectManager.OpenPanelAfterReturn();

        BattleData.ResetBattleMode();

        SceneManager.LoadScene("Town");
    }

    void StartTutorial3()
    {
        if (tutorial3PlayerCard == null || tutorial3EnemyJudgmentCard == null || tutorial3WolfCard == null || tutorial3PlayerJudgmentCard == null)
        {
            return;
        }

        if (CardManager.Inst == null || TurnManager.Inst == null || EntityManager.Inst == null)
        {
            return;
        }

        CardManager.Inst.SetupDecks();

        EntityManager.Inst.SetSummonOwner(null);

        bool success = EntityManager.Inst.SpawnEntity(true, tutorial3PlayerCard, EntityManager.Inst.MyDeckSpawnPos);

        EntityManager.Inst.ClearSummonOwner();

        if (!success)
            return;

        tutorial3EnemyJudgmentGraveCount = 0;
        tutorial3Step = Tutorial3Step.AttackEnemyDeck;

        if (BattleTutorialHighlight.Inst != null)
        {
            BattleTutorialHighlight.Inst.DimAll();

            Entity playerEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && x.Data == tutorial3PlayerCard);

            if (playerEntity != null)
            {
                BattleTutorialHighlight.Inst.Highlight(playerEntity.gameObject);
            }

            Entity enemyBoss = EntityManager.Inst.OtherBossEntity;

            if (enemyBoss != null)
            {
                BattleTutorialHighlight.Inst.Highlight(enemyBoss.gameObject);
            }

            BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetPicker);

            BattleTutorialHighlight.Inst.Highlight(EntityManager.Inst.TutorialTargetArrow);

            if (BattleTutorialUI.Inst != null && BattleTutorialUI.Inst.GuidePanel != null)
            {
                BattleTutorialHighlight.Inst.Highlight(BattleTutorialUI.Inst.GuidePanel);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("하수인으로 적을 직접 공격하세요.");
        }

        StartCoroutine(TurnManager.Inst.StartTutorialGameCo());
    }

    public void OnDeckToGraveTriggered(CardDataSO data, bool isMineDeck, Entity deckAttacker)
    {
        if (!IsActive)
            return;

        if (BattleData.battleMode != BattleMode.Tutorial3)
            return;

        if (tutorial3Step != Tutorial3Step.AttackEnemyDeck)
            return;

        if (isMineDeck)
            return;

        if (data != tutorial3EnemyJudgmentCard)
            return;

        tutorial3EnemyJudgmentGraveCount++;

        if (tutorial3EnemyJudgmentGraveCount < 6)
            return;

        tutorial3Step = Tutorial3Step.EnemyJudgmentTriggered;

        StartCoroutine(Tutorial3AfterEnemyJudgmentCo());
    }

    IEnumerator Tutorial3AfterEnemyJudgmentCo()
    {
        int safety = 100;

        while (safety-- > 0)
        {
            int wolfCount = 0;

            foreach (Entity entity in EntityManager.Inst.OtherEntities)
            {
                if (entity == null)
                    continue;

                if (entity.isDie)
                    continue;

                if (entity.isBossOrEmpty)
                    continue;

                if (entity.Data == tutorial3WolfCard)
                {
                    wolfCount++;
                }
            }

            if (wolfCount >= 6)
                break;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.8f);

        tutorial3Step = Tutorial3Step.OpenEnemyGrave;

        if (BattleTutorialHighlight.Inst != null)
        {
            Entity playerEntity = EntityManager.Inst.MyEntities.Find(x => x != null && !x.isBossOrEmpty && !x.isDie && x.Data == tutorial3PlayerCard);

            if (playerEntity != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(playerEntity.gameObject);
            }

            Entity enemyBoss = EntityManager.Inst.OtherBossEntity;

            if (enemyBoss != null)
            {
                BattleTutorialHighlight.Inst.DimTarget(enemyBoss.gameObject);
            }
        }

        if (BattleTutorialUI.Inst != null)
        {
            BattleTutorialUI.Inst.ShowGuide("심판 능력으로 하수인들이 소환되었습니다.\n\n상대의 묘지를 확인하세요.");
        }
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}