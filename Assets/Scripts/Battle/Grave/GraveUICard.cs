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

    public void Setup(CardDataSO data)
    {
        character.sprite = data.sprite;
        nameTMP.text = data.cardName;
        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();

        if (manaTMP != null)
            manaTMP.text = data.manaCost.ToString();

        if (descriptionTMP != null)
            descriptionTMP.text = data.description;
    }
}