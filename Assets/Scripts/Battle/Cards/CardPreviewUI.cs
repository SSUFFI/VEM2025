using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    [SerializeField] Image cardFrame;
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text manaTMP;
    [SerializeField] TMP_Text descriptionTMP;

    [Header("Keyword")]
    [SerializeField] GameObject keywordObject;
    [SerializeField] TMP_Text keywordTMP;

    public void Setup(CardDataSO data)
    {
        if (data == null) return;

        if (character != null)
            character.sprite = data.sprite;

        if (nameTMP != null)
            nameTMP.text = data.cardName;

        if (attackTMP != null)
            attackTMP.text = data.attack.ToString();

        if (healthTMP != null)
            healthTMP.text = data.health.ToString();

        if (manaTMP != null)
            manaTMP.text = data.manaCost.ToString();

        if (descriptionTMP != null)
            descriptionTMP.text = data.description;

        RefreshKeyword(data);
    }

    void RefreshKeyword(CardDataSO data)
    {
        if (keywordObject == null || keywordTMP == null)
            return;

        if (data.keywords != null && data.keywords.Count > 0)
        {
            keywordObject.SetActive(true);
            keywordTMP.text = string.Join(" / ", data.keywords);
        }
        else
        {
            keywordObject.SetActive(false);
        }
    }
}