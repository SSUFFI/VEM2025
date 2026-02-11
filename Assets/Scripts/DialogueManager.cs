using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;


public class DialogueManager : MonoBehaviour
{
    bool isChoosing = false;

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

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button[] choiceButtons;

    [System.Serializable]
    public class Line
    {
        public Side speaker;
        public string speakerName;
        [TextArea] public string text;
        public Sprite portrait;

        public Sprite leftIllustration;
        public Sprite rightIllustration;

        public bool hasChoice;
        public Choice[] choices;

        public int nextIndex = -1;
    }

    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public int nextIndex;
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

        ShowLine();
        UpdateNextText();
    }

    void OnClickDialogueBox()
    {
        if (isChoosing)
            return;

        if (isTyping)
        {
            SkipTyping();
            return;
        }

        if (canClose)
        {
            EndDialogue();
            return;
        }

        var currentLine = lines[index];

        if (currentLine.nextIndex == -999)
        {
            canClose = true;
            UpdateNextText();
            return;
        }

        else if (currentLine.nextIndex == -1)
        {
            index++;
        }

        else
        {
            index = currentLine.nextIndex;
        }

        ShowLine();
        UpdateNextText();
    }



    void ShowLine()
    {
        var line = lines[index];

        nameText.text = line.speakerName;

        if (line.leftIllustration != null)
            leftCharacterImage.sprite = line.leftIllustration;

        if (line.rightIllustration != null)
            rightCharacterImage.sprite = line.rightIllustration;

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

        StartTyping(line.text);

        if (line.hasChoice)
        {
            ShowChoices(line);
        }
        else
        {
            choicePanel.SetActive(false);
        }
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

        if (canClose)
            nextText.text = "대화 종료";
        else
            nextText.text = "다음";
    }

    void EndDialogue()
    {
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

    void ShowChoices(Line line)
    {
        isChoosing = true;
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < line.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);

                int choiceIndex = i; // 클로저 방지

                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    line.choices[i].choiceText;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    SelectChoice(line.choices[choiceIndex].nextIndex);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectChoice(int nextIndex)
    {
        isChoosing = false;
        choicePanel.SetActive(false);

        index = nextIndex;
        canClose = false;

        ShowLine();
        UpdateNextText();
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
