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