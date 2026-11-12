using UnityEngine;

public class EventClearButton : MonoBehaviour
{
    public void OnClickClear()
    {
        NodeMapManager.Inst.ClearSelectedNode();
        NodeMapManager.Inst.CloseAllEventPanels();
    }
}