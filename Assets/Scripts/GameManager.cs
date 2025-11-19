using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] NotificationPanel notificationPanel;
    WaitForSeconds delay2 = new WaitForSeconds(2);
    

    void Start()
    {
        StartGame();
    }


    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManager.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManager.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            TurnManager.Inst.EndTurn();

        if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.TryPutCard(false);
    }

    public void StartGame()
    {
        StartCoroutine(TurnManager.Inst.StartGameCo());
    }

    public void Notification(string message)
    {
        notificationPanel.show(message);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        yield return delay2;
    }

}
