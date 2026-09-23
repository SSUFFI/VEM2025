using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GraveUI : MonoBehaviour
{
    public static GraveUI Inst;

    [SerializeField] GameObject gravePanel;
    [SerializeField] Transform content;
    [SerializeField] GameObject cardPrefab;

    [Header("Grave Highlight Targets")]
    [SerializeField] GameObject myGraveHighlightTarget;
    [SerializeField] GameObject enemyGraveHighlightTarget;

    public GameObject MyGraveHighlightTarget => myGraveHighlightTarget;
    public GameObject EnemyGraveHighlightTarget => enemyGraveHighlightTarget;

    bool isOpen = false;
    public bool IsOpen => isOpen;

    void Awake() => Inst = this;

    public void OpenGrave(bool isMine)
    {
        gravePanel.SetActive(true);
        isOpen = true;

        foreach (Transform t in content)
            Destroy(t.gameObject);

        var list = isMine ? GraveManager.Inst.myGrave : GraveManager.Inst.enemyGrave;

        foreach (var data in list)
        {
            var obj = Instantiate(cardPrefab, content);
            obj.GetComponent<GraveUICard>().Setup(data, isMine);
        }

        if (BattleTutorialManager.Inst != null && BattleTutorialManager.Inst.IsActive)
        {
            BattleTutorialManager.Inst.OnGraveOpened(isMine);
        }
    }

    public void Close()
    {
        gravePanel.SetActive(false);
        isOpen = false;
    }

}
