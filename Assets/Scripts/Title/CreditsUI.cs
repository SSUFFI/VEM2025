using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;

    void Start()
    {
        creditsPanel.SetActive(false);
    }

    void Update()
    {
        if (!creditsPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            CloseCredits();
        }
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
}