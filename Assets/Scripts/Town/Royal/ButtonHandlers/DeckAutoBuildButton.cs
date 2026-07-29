using UnityEngine;

public class DeckAutoBuildButton : MonoBehaviour
{
    public void OnClickAutoBuild()
    {
        if (DeckEditManager.Inst == null)
        {
            Debug.LogWarning("DeckEditManager.Inst가 없습니다.");
            return;
        }

        DeckEditManager.Inst.AutoBuildRandomDeck();
    }
}