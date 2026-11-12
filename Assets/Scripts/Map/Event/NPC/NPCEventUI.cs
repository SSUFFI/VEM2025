
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCEventUI : MonoBehaviour
{
    [Header("질문 데이터")]
    [SerializeField] private NPCQuestionDataSO[] questions;

    [Header("질문 텍스트")]
    [SerializeField] private TMP_Text questionText;

    [Header("선택지 3개")]
    [SerializeField] private Button[] answerButtons = new Button[3];

    [SerializeField] private TMP_Text[] answerTexts = new TMP_Text[3];

    [Header("잠긴 네 번째 선택지")]
    [SerializeField] private Button lockedButton;

    [Header("획득 증표 표시")]
    [SerializeField] private Image rewardTokenImage;

    [Header("나가기 버튼")]
    [SerializeField] private GameObject exitButton;

    private NPCQuestionDataSO currentQuestion;
    private bool answered;

    private void OnEnable()
    {
        InitializeEvent();
    }

    private void InitializeEvent()
    {
        answered = false;

        if (rewardTokenImage != null)
        {
            rewardTokenImage.sprite = null;
            rewardTokenImage.gameObject.SetActive(false);
        }

        currentQuestion = null;

        if (exitButton != null)
            exitButton.SetActive(false);

        if (lockedButton != null)
        {
            lockedButton.gameObject.SetActive(true);
            lockedButton.interactable = false;
        }

        foreach (Button button in answerButtons)
        {
            if (button != null)
                button.gameObject.SetActive(true);
        }

        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("NPC 질문이 등록되지 않았습니다.");
            return;
        }

        currentQuestion = questions[
            Random.Range(0, questions.Length)];

        if (currentQuestion == null ||
            currentQuestion.answers == null ||
            currentQuestion.answers.Length != 3)
        {
            Debug.LogError("NPC 질문의 답변 데이터가 올바르지 않습니다.");
            return;
        }

        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < 3; i++)
        {
            int index = i;

            answerTexts[i].text =
                currentQuestion.answers[i].answerText;

            answerButtons[i].interactable = true;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(
                () => SelectAnswer(index));
        }
    }

    private void SelectAnswer(int index)
    {
        if (answered || currentQuestion == null)
            return;

        NPCAnswerData answer = currentQuestion.answers[index];

        if (EventTokenManager.Instance == null)
        {
            Debug.LogError("EventTokenManager가 없습니다.");
            return;
        }

        if (answer.rewardToken == null ||
            answer.rewardToken.source != TokenSource.NPC)
        {
            Debug.LogError("NPC 전용 보상 증표가 올바르지 않습니다.");
            return;
        }

        if (!EventTokenManager.Instance.AddToken(answer.rewardToken))
        {
            Debug.LogWarning("증표를 획득하지 못했습니다.");
            return;
        }

        answered = true;

        if (rewardTokenImage != null)
        {
            rewardTokenImage.sprite = answer.rewardToken.tokenIcon;
            rewardTokenImage.color = Color.white;
            rewardTokenImage.gameObject.SetActive(true);
        }

        questionText.text = answer.npcComment;

        foreach (Button button in answerButtons)
            button.gameObject.SetActive(false);

        if (lockedButton != null)
            lockedButton.gameObject.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(true);
    }
}
