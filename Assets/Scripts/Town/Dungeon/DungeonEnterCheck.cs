using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonEnterCheck : MonoBehaviour
{
    public Button dungeonButton;
    public TMP_Text warningText;

    void Update()
    {
        CheckDeck();
    }

    void CheckDeck()
    {
        if (DeckEditManager.Inst == null)
            return;

        int count =
            DeckEditManager.Inst.savedDeck.Count;

        int max =
            DeckEditManager.Inst.maxDeckSize;

        if (count < max)
        {
            dungeonButton.interactable = false;

            if (warningText != null)
            {
                warningText.gameObject.SetActive(true);
                warningText.text =
                    $"덱을 {max}장 채워주세요";
            }
        }
        else
        {
            dungeonButton.interactable = true;

            if (warningText != null)
                warningText.gameObject.SetActive(false);
        }
    }
}