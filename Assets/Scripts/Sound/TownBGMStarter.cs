using UnityEngine;

public class TownBGMStarter : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Inst.PlayTownBGM();
    }
}