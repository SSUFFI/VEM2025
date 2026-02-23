using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField][Tooltip("시작 턴 모드를 정합니다")] ETurnMode eTurnMode;
    [SerializeField][Tooltip("빠른 카드 배분")] bool fastMode;
    [SerializeField][Tooltip("시작 카드 개수를 정합니다")] int startCardCount;

    [Header("Properties")]
    public bool isLoading;
    public bool myTurn;

    [Header("Mana")]
    public int myMaxMana = 0;
    public int myCurMana = 0;
    public int otherMaxMana = 0;
    public int otherCurMana = 0;
    public int maxManaCap = 10;

    public static event Action OnManaChanged;

    enum ETurnMode { Random, My, Other }
    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    void GameSetup()
    {
        if (fastMode)
            delay05 = new WaitForSeconds(0.3f);

        switch(eTurnMode)
        {
            case ETurnMode.Random:
                myTurn = Random.Range(0, 2) == 0;
                break;
            case ETurnMode.My:
                myTurn = true;
                break;
            case ETurnMode.Other:
                myTurn = false;
                break;
        }
    }

    public IEnumerator StartGameCo()
    {
        GameSetup();
        isLoading = true;

        for (int i = 0; i < startCardCount; i++)
        {
            yield return delay05;
            OnAddCard?.Invoke(false);
            yield return delay05;
            OnAddCard?.Invoke(true);
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;
        if (myTurn)
            BattleGameManager.Inst.Notification("나의 턴");

        ApplyTurnStartMana(myTurn);

        yield return delay07;
        OnAddCard?.Invoke(myTurn);
        yield return delay07;
        isLoading = false;
        OnTurnStarted?.Invoke(myTurn);
    }

    void ApplyTurnStartMana(bool isMyTurnNow)
    {
        if (isMyTurnNow)
        {
            myMaxMana = Mathf.Min(maxManaCap, myMaxMana + 1);
            myCurMana = myMaxMana;
        }
        else
        {
            otherMaxMana = Mathf.Min(maxManaCap, otherMaxMana + 1);
            otherCurMana = otherMaxMana;
        }

        OnManaChanged?.Invoke();
    }

    public bool CanPayMana(bool isMine, int cost)
    {
        if (cost <= 0) return true;
        return isMine ? myCurMana >= cost : otherCurMana >= cost;
    }

    public void PayMana(bool isMine, int cost)
    {
        if (cost <= 0) return;

        if (isMine) myCurMana -= cost;
        else otherCurMana -= cost;

        OnManaChanged?.Invoke();
    }

    public void EndTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(StartTurnCo());
    }
}
