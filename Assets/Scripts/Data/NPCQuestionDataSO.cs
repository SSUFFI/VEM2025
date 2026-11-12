
using UnityEngine;

[System.Serializable]
public class NPCAnswerData
{
    [TextArea(2, 4)]
    public string answerText;

    public TokenDataSO rewardToken;

    [TextArea(2, 5)]
    public string npcComment;
}

[CreateAssetMenu(fileName = "NPCQuestionDataSO", menuName = "Scriptable Object/NPCQuestionDataSO")]
public class NPCQuestionDataSO : ScriptableObject
{
    [TextArea(3, 6)]
    public string questionText;

    public NPCAnswerData[] answers = new NPCAnswerData[3];
}
