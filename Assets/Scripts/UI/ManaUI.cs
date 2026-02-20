using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaUI : MonoBehaviour
{
    [Header("My Mana")]
    [SerializeField] TMP_Text myManaTMP;

    [Header("Other Mana")]
    [SerializeField] TMP_Text otherManaTMP;

    void Start()
    {
        UpdateManaUI();
        TurnManager.OnManaChanged += UpdateManaUI;
    }

    void OnDestroy()
    {
        TurnManager.OnManaChanged -= UpdateManaUI;
    }

    void UpdateManaUI()
    {
        var tm = TurnManager.Inst;

        myManaTMP.text = $"{tm.myCurMana} / {tm.myMaxMana}";
        otherManaTMP.text = $"{tm.otherCurMana} / {tm.otherMaxMana}";
    }
}