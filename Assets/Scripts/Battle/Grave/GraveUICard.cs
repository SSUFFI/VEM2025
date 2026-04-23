using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraveUICard : MonoBehaviour
{
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text manaTMP;
    [SerializeField] TMP_Text descriptionTMP;

    CardDataSO data;

    public void Setup(CardDataSO data)
    {
        this.data = data;

        character.sprite = data.sprite;
        nameTMP.text = data.cardName;
        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();

        if (manaTMP != null)
            manaTMP.text = data.manaCost.ToString();

        if (descriptionTMP != null)
            descriptionTMP.text = data.description;
    }

    public void OnClick()
    {
        if (CardPreviewManager.Inst == null) return;
        if (data == null) return;

        CardPreviewManager.Inst.Show(data);
    }
}