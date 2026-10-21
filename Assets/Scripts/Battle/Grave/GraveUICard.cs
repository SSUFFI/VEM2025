
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GraveUICard : MonoBehaviour, IPointerClickHandler
{
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

    CardDataSO data;
    bool isMine;

    public void Setup(CardDataSO data, bool isMine)
    {
        this.data = data;
        this.isMine = isMine;

        character.sprite = data.sprite;

        if (raceImage != null)
        {
            raceImage.sprite = data.raceSprite;
            raceImage.gameObject.SetActive(data.raceSprite != null);
        }

        nameTMP.text = data.cardName;
        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();

        if (manaTMP != null)
            manaTMP.text = data.manaCost.ToString();

        SetupDescriptions(data);
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

    void SetDescription(TMP_Text text, GameObject gem, string description)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Application.isMobilePlatform)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;
        }
        else
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
        }

        ShowPreview();
    }

    public void ShowPreview()
    {
        if (CardPreviewManager.Inst == null) return;
        if (data == null) return;

        CardPreviewManager.Inst.Show(data);

        if (BattleTutorialManager.Inst != null && BattleTutorialManager.Inst.IsActive)
        {
            BattleTutorialManager.Inst.OnGraveCardPreviewed(data, isMine);
        }
    }
}
