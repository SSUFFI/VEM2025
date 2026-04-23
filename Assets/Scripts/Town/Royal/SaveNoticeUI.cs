using System.Collections;
using UnityEngine;

public class SaveNoticeUI : MonoBehaviour
{
    [SerializeField] GameObject noticeObj;

    Coroutine routine;

    void Start()
    {
        noticeObj.SetActive(false);
    }

    void OnEnable()
    {
        noticeObj.SetActive(false);
    }

    public void ShowNotice()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        noticeObj.SetActive(true);

        yield return new WaitForSeconds(1f);

        noticeObj.SetActive(false);
    }
}