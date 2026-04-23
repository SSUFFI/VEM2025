using UnityEngine;

public class ExploreNoteUI : MonoBehaviour
{
    [SerializeField] GameObject note1;
    [SerializeField] GameObject note2;
    [SerializeField] GameObject note3;

    public void OpenNote1()
    {
        if (!note1.activeSelf)
            note1.SetActive(true);
    }

    public void OpenNote2()
    {
        if (!note2.activeSelf)
            note2.SetActive(true);
    }

    public void OpenNote3()
    {
        if (!note3.activeSelf)
            note3.SetActive(true);
    }
}