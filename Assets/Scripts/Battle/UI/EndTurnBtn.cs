using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndTurnBtn : MonoBehaviour
{
    [SerializeField] Sprite active;
    [SerializeField] Sprite inactive;
    [SerializeField] TMP_Text btnText;

    Button button;
    Image image;

    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        Setup(false);

        TurnManager.OnTurnStarted += Setup;
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= Setup;
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