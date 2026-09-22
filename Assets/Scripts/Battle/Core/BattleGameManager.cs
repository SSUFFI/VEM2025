using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    public static BattleGameManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] NotificationPanel notificationPanel;
    WaitForSeconds delay2 = new WaitForSeconds(2);

    void Start()
    {
        if (BattleData.IsBattleTutorial)
        {
            if (BattleTutorialManager.Inst != null)
            {
                BattleTutorialManager.Inst.StartTutorial();
                return;
            }

            Debug.LogError(
                "BattleTutorialManager가 Battle 씬에 없습니다.");

            return;
        }

        StartGame();
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        // 1번 : 내 드로우
        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManager.OnAddCard?.Invoke(true);

        // 2번 : 적 드로우
        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManager.OnAddCard?.Invoke(false);

        // 3번 : 내 덱 맨 위 카드 1장 묘지
        if (Input.GetKeyDown(KeyCode.Keypad3))
            CardManager.Inst.DamageDeck(1, true, null);

        // 4번 : 적 덱 맨 위 카드 1장 묘지
        if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.DamageDeck(1, false, null);
    }

    public void StartGame()
    {
        StartCoroutine(TurnManager.Inst.StartGameCo());
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
}