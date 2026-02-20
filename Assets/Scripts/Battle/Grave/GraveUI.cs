using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GraveUI : MonoBehaviour
{
    public static GraveUI Inst;

    [SerializeField] GameObject gravePanel;
    [SerializeField] Transform content;
    [SerializeField] GameObject cardPrefab;

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
            obj.GetComponent<GraveUICard>().Setup(data);
        }
    }

    public void Close()
    {
        gravePanel.SetActive(false);
        isOpen = false;
    }

}
