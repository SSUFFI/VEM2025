using UnityEngine;

public class BattleBGMStarter : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Inst.PlayBattleBGM();
    }
}