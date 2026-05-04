using UnityEngine;

public class DeckClearButton : MonoBehaviour
{
    public void OnClickClear()
    {
        if (DeckEditManager.Inst != null)
        {
            DeckEditManager.Inst.ClearDeck();

            if (DeckListUI.Inst != null)
                DeckListUI.Inst.Refresh();
        }
    }
}