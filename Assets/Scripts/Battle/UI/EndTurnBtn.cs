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
        if (BattleRelicUI.Inst != null &&
            BattleRelicUI.Inst.IsTargeting)
        {
            button.interactable = false;

            image.sprite = inactive;

            btnText.color = new Color32(55, 55, 55, 255);

            return;
        }

        bool myTurn =
            TurnManager.Inst != null &&
            TurnManager.Inst.myTurn;

        Setup(myTurn);
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