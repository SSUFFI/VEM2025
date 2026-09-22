using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndTurnBtn : MonoBehaviour
{
    public static EndTurnBtn Inst;

    [SerializeField] Sprite active;
    [SerializeField] Sprite inactive;
    [SerializeField] TMP_Text btnText;

    Button button;
    Image image;

    void Start()
    {
        Inst = this;

        button = GetComponent<Button>();
        image = GetComponent<Image>();

        Setup(false);

        TurnManager.OnTurnStarted += Setup;
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= Setup;

        if (Inst == this)
            Inst = null;
    }

    void Update()
    {
        bool canClick =
            TurnManager.Inst != null &&
            TurnManager.Inst.myTurn &&
            !TurnManager.Inst.isLoading;

        if (BattleRelicUI.Inst != null &&
            BattleRelicUI.Inst.IsTargeting)
        {
            canClick = false;
        }

        if (canClick &&
            BattleTutorialManager.Inst != null &&
            BattleTutorialManager.Inst.IsActive)
        {
            canClick =
                BattleTutorialManager.Inst.CanEndTurn();
        }

        Setup(canClick);
    }

    public void Setup(bool isActive)
    {
        if (button == null || image == null)
            return;

        image.sprite = isActive ? active : inactive;
        button.interactable = isActive;

        btnText.color = isActive
            ? new Color32(255, 195, 90, 255)
            : new Color32(55, 55, 55, 255);
    }
}