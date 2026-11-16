using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    public static BattleGameManager Inst { get; private set; }

    [SerializeField]
    private NotificationPanel notificationPanel;

    [SerializeField]
    private SlateBattleIntroUI slateIntro;

    private bool gameStarted;
    private SlateBattleEffectState slateEffects;

    private readonly WaitForSeconds delay2 =
        new WaitForSeconds(2f);

    public SlateBattleEffectState SlateEffects =>
        GameResultManager.IsBattleFinished
            ? null
            : slateEffects;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        if (BattleData.IsBattleTutorial)
        {
            if (BattleTutorialManager.Inst != null)
            {
                BattleTutorialManager.Inst.StartTutorial();
                return;
            }

            Debug.LogError(
                "BattleTutorialManager가 전투 씬에 없습니다.");

            return;
        }

        StartGame();
    }

    private void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    private void InputCheatKey()
    {
        if (GameResultManager.IsBattleFinished)
            return;

        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManager.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManager.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            CardManager.Inst.DamageDeck(1, true, null);

        if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.DamageDeck(1, false, null);
    }

    public void StartGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;
        TurnManager.Inst.isLoading = true;

        StartCoroutine(StartGameWithSlate());
    }

    private IEnumerator StartGameWithSlate()
    {
        // 다른 매니저의 Start에서 덱 초기화와 이벤트 구독을 마치도록 대기.
        yield return null;

        while (FadeManager.Instance != null &&
               FadeManager.Instance.IsTransitioning)
        {
            yield return null;
        }

        SlateDataSO reward = SlateRunState.Current;

        bool isTargetBossBattle =
            BattleData.battleMode == BattleMode.Normal &&
            !BattleData.isTutorialBattle &&
            BattleData.selectedNodeType == NodeType.Boss &&
            reward != null;

        if (isTargetBossBattle)
        {
            Action apply = () =>
            {
                slateEffects = new SlateBattleEffectState(
                    reward.effectType,
                    reward.effectValue);
            };

            if (reward.showRune)
            {
                if (slateIntro == null)
                    slateIntro = GetComponent<SlateBattleIntroUI>();

                if (slateIntro == null)
                {
                    slateIntro =
                        gameObject.AddComponent<SlateBattleIntroUI>();
                }

                TMP_Text label = notificationPanel != null
                    ? notificationPanel.GetComponentInChildren<TMP_Text>(true)
                    : null;

                TMP_FontAsset font =
                    label != null ? label.font : null;

                yield return slateIntro.Play(
                    reward,
                    apply,
                    font);
            }
            else
            {
                apply();
            }
        }

        if (!GameResultManager.IsBattleFinished)
            yield return TurnManager.Inst.StartGameCo();
    }

    public void EndSlateEffects()
    {
        slateEffects = null;

        if (EntityManager.Inst != null)
        {
            foreach (Entity entity in EntityManager.Inst.MyEntities)
            {
                if (entity != null)
                    entity.RemoveSlateBonus();
            }
        }

        if (CardManager.Inst != null)
            CardManager.Inst.RefreshHandManaCosts();
    }

    public void Notification(string message)
    {
        notificationPanel.show(message);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        yield return delay2;
    }

    private void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}