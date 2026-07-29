using UnityEngine;

public class DeckSaveButton : MonoBehaviour
{
    public void OnClickSave()
    {
        if (DeckEditManager.Inst != null)
            DeckEditManager.Inst.SaveDeck();
    }
}