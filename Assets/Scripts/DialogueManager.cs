using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class DialogueManager : MonoBehaviour
{
    [Header("Root")]
    public GameObject dialoguePanel;
    public Button dialogueBoxButton;

    [Header("Characters")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [Header("Portrait")]
    public Image portraitImage;

    [Header("Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nextText; // "다음" / "대화 종료"

    [Header("Typing")]
    [Range(0.02f, 0.3f)]
    public float wordDelay = 0.08f;

    [Header("Dialogue Data")]
    public Line[] lines;

    [System.Serializable]
    public class Line
    {
        public Side speaker;
        public string speakerName;
        [TextArea] public string text;
        public Sprite portrait;
    }

    public enum Side { Left, Right }

    int index;
    bool canClose;

    Coroutine typingCo;
    bool isTyping;
    string currentFullText;

    void Awake()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (dialogueBoxButton != null)
        {
            dialogueBoxButton.onClick.RemoveListener(OnClickDialogueBox);
            dialogueBoxButton.onClick.AddListener(OnClickDialogueBox);
        }
    }

    public void StartDialogue()
    {
        if (lines == null || lines.Length == 0) return;

        dialoguePanel.SetActive(true);
        index = 0;
        canClose = false;

        ShowLine();          // 첫 줄 세팅 + 타이핑 시작
        UpdateNextText();    // "다음"
    }

    void OnClickDialogueBox()
    {
        // 1) 타이핑 중이면 -> 즉시 완성(스킵)
        if (isTyping)
        {
            SkipTyping();
            return;
        }

        // 2) 타이핑 끝난 상태면 기존 로직(다음/종료)
        if (canClose)
        {
            EndDialogue();
            return;
        }

        index++;

        // 마지막 줄이면 canClose 상태로 전환 (3번째 줄에서 종료 안내)
        if (index >= lines.Length - 1)
        {
            index = lines.Length - 1;
            canClose = true;
        }

        ShowLine();       // 다음 줄 보여주기 + 타이핑 시작
        UpdateNextText(); // 마지막이면 "대화 종료"
    }

    void ShowLine()
    {
        var line = lines[index];

        // 이름
        nameText.text = line.speakerName;

        // 말하는 쪽 밝게
        if (line.speaker == Side.Left)
        {
            SetAlpha(leftCharacterImage, 1f);
            SetAlpha(rightCharacterImage, 0.3f);
        }
        else
        {
            SetAlpha(leftCharacterImage, 0.3f);
            SetAlpha(rightCharacterImage, 1f);
        }

        // 초상화
        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.enabled = true;
                portraitImage.sprite = line.portrait;
                portraitImage.preserveAspect = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }

        // 타이핑 시작
        StartTyping(line.text);
    }

    void StartTyping(string fullText)
    {
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        currentFullText = fullText;
        dialogueText.text = "";
        isTyping = true;

        typingCo = StartCoroutine(TypingWordsCoroutine(fullText));
    }

    IEnumerator TypingWordsCoroutine(string fullText)
    {
        var tokens = TokenizeByWhitespace(fullText);

        for (int i = 0; i < tokens.Count; i++)
        {
            dialogueText.text += tokens[i];
            yield return new WaitForSeconds(wordDelay);
        }

        isTyping = false;
        typingCo = null;
    }

    void SkipTyping()
    {
        // 타이핑 중이면 즉시 완성
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        dialogueText.text = currentFullText;
        isTyping = false;
    }

    void UpdateNextText()
    {
        if (nextText == null) return;
        nextText.text = canClose ? "대화 종료" : "다음";
    }

    void EndDialogue()
    {
        // 혹시 타이핑 중 종료되면 정리
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }
        isTyping = false;

        dialoguePanel.SetActive(false);
    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        c.a = a;
        img.color = c;
    }

    List<string> TokenizeByWhitespace(string text)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(text)) return result;

        var sb = new StringBuilder();
        bool inWhite = char.IsWhiteSpace(text[0]);

        for (int i = 0; i < text.Length; i++)
        {
            bool isWhite = char.IsWhiteSpace(text[i]);

            // 상태가 바뀌면 지금까지 누적한 토큰 저장
            if (isWhite != inWhite && sb.Length > 0)
            {
                result.Add(sb.ToString());
                sb.Clear();
                inWhite = isWhite;
            }

            sb.Append(text[i]);
        }

        if (sb.Length > 0)
            result.Add(sb.ToString());

        return result;
    }
}
