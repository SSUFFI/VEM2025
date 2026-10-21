
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    [SerializeField] Image cardFrame;
    [SerializeField] Image character;
    [SerializeField] Image raceImage;

    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text manaTMP;

    [Header("Descriptions")]
    [SerializeField] TMP_Text descriptionTopTMP;
    [SerializeField] TMP_Text descriptionMiddleTMP;
    [SerializeField] TMP_Text descriptionBottomTMP;
    [SerializeField] TMP_Text descriptionFloorTMP;

    [Header("Description Gems")]
    [SerializeField] GameObject topGem;
    [SerializeField] GameObject middleGem;
    [SerializeField] GameObject bottomGem;

    [Header("Keyword")]
    [SerializeField] GameObject keywordObject;
    [SerializeField] TMP_Text keywordTMP;

    public void Setup(CardDataSO data)
    {
        if (data == null) return;

        if (character != null)
            character.sprite = data.sprite;

        if (raceImage != null)
        {
            raceImage.sprite = data.raceSprite;
            raceImage.gameObject.SetActive(data.raceSprite != null);
        }

        if (nameTMP != null)
            nameTMP.text = data.cardName;

        if (attackTMP != null)
            attackTMP.text = data.attack.ToString();

        if (healthTMP != null)
            healthTMP.text = data.health.ToString();

        if (manaTMP != null)
            manaTMP.text = data.manaCost.ToString();

        SetupDescriptions(data);

        RefreshKeyword(data);
    }

    void SetupDescriptions(CardDataSO data)
    {
        SetDescription(
            descriptionTopTMP,
            topGem,
            data.descriptionTop
        );

        SetDescription(
            descriptionMiddleTMP,
            middleGem,
            data.descriptionMiddle
        );

        SetDescription(
            descriptionBottomTMP,
            bottomGem,
            data.descriptionBottom
        );

        SetDescription(
            descriptionFloorTMP,
            null,
            data.descriptionFloor
        );
    }

    void SetDescription(
        TMP_Text text,
        GameObject gem,
        string description)
    {
        bool hasText = !string.IsNullOrWhiteSpace(description);

        if (text != null)
        {
            text.text = hasText ? description : "";
            text.gameObject.SetActive(hasText);
        }

        if (gem != null)
            gem.SetActive(hasText);
    }

    void RefreshKeyword(CardDataSO data)
    {
        if (keywordObject == null || keywordTMP == null)
            return;

        if (data.keywords != null && data.keywords.Count > 0)
        {
            keywordObject.SetActive(true);
            keywordTMP.text = string.Join("\n\n", data.keywords);
        }
        else
        {
            keywordObject.SetActive(false);
        }
    }
}
