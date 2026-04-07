using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSaveButton : MonoBehaviour
{
    public void OnClickSave()
    {
        if (DeckEditManager.Inst != null)
        {
            DeckEditManager.Inst.SaveDeck();
        }
    }
}