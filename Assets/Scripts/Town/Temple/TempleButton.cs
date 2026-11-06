using UnityEngine;

public class TempleButton : MonoBehaviour
{
    public void OnClickTemple()
    {
        TempleUIManager.Inst.OpenMainPanel();
    }
}