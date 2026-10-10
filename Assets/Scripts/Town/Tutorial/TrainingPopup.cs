using UnityEngine;
using System.Collections;

public class TrainingPopup : MonoBehaviour
{
    public static TrainingPopup Inst;

    [Header("Popup")]
    [SerializeField] GameObject popupPanel;

    bool canClose;

    void Awake()
    {
        Inst = this;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    void Update()
    {
        if (!canClose)
            return;

        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1))
        {
            Close();
        }
    }

    public void Show()
    {
        if (popupPanel == null)
            return;

        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();

        canClose = false;

        StartCoroutine(EnableCloseNextFrame());
    }

    IEnumerator EnableCloseNextFrame()
    {
        yield return null;
        canClose = true;
    }

    public void Close()
    {
        canClose = false;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}