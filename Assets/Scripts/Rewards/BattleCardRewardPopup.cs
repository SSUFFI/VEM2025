using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleCardRewardPopup : MonoBehaviour
{
    [Header("팝업 배경 오브젝트")]
    [SerializeField] private GameObject popupRoot;

    [Header("텍스트")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("카드 일러스트 ? 보스전은 최대 2개")]
    [SerializeField] private Image[] cardImages = new Image[2];

    [Header("닫기 또는 확인 버튼")]
    [SerializeField] private Button closeButton;

    private bool showing;

    private void OnEnable()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (popupRoot != null)
            popupRoot.SetActive(false);

        StartCoroutine(ShowAfterFade());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);

        if (popupRoot != null)
            popupRoot.SetActive(false);

        showing = false;
    }

    private IEnumerator ShowAfterFade()
    {
        // 다른 매니저의 Start와 씬 페이드 시작을 기다립니다.
        yield return null;

        while (FadeManager.Instance != null &&
               FadeManager.Instance.IsTransitioning)
        {
            yield return null;
        }

        string scene =
            SceneManager.GetActiveScene().name;

        if (scene != "Map" && scene != "Town")
            yield break;

        if (!BattleCardRewards.TryGetNotice(
                scene == "Town",
                out List<BattleCardRewardEntry> entries))
        {
            yield break;
        }

        if (popupRoot == null || descriptionText == null)
        {
            Debug.LogError(
                "BattleCardRewardPopup: Popup Root와 " +
                "Description Text를 연결하세요.");

            yield break;
        }

        if (titleText != null)
        {
            titleText.text = scene == "Town"
                ? "보스 처치 보상"
                : "전리품 획득";
        }

        StringBuilder text = new StringBuilder();
        int totalCopies = 0;

        foreach (BattleCardRewardEntry entry in entries)
        {
            if (text.Length > 0)
            {
                text.AppendLine();
                text.AppendLine();
            }

            text.AppendLine(entry.card.cardName);

            if (entry.newlyUnlocked)
            {
                text.AppendLine(
                    "왕국 카드 목록에 새롭게 해금되었습니다." 
                   );
            }
            else
            {
                text.AppendLine(
                    "이미 해금된 카드입니다. 중복 보상 +30G");
            }

            if (entry.temporaryCopies > 0)
            {
                text.Append("이번 던전의 전투 덱에 ")
                    .Append(entry.temporaryCopies)
                    .Append("장 추가");

                totalCopies += entry.temporaryCopies;
            }
/*            else
            {
                text.Append(
                    "영구 해금 보상: 임시 덱에는 추가되지 않습니다.");
            }*/
        }

/*        if (totalCopies > 0)
        {
            text.AppendLine()S
                .AppendLine()
                .Append("다음 전투 시작 덱/최대 체력 +")
                .Append(totalCopies)
                .Append(" (초기 패 배분 전 기준)");
        }*/

        descriptionText.text = text.ToString();

        if (cardImages != null)
        {
            for (int i = 0; i < cardImages.Length; i++)
            {
                Image image = cardImages[i];

                if (image == null)
                    continue;

                image.sprite = i < entries.Count
                    ? entries[i].card.sprite
                    : null;

                image.enabled = image.sprite != null;
                image.preserveAspect = true;
            }
        }

        showing = true;

        popupRoot.SetActive(true);
        popupRoot.transform.SetAsLastSibling();
    }

    public void Close()
    {
        if (!showing)
            return;

        showing = false;

        BattleCardRewards.DismissNotice();

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }
}