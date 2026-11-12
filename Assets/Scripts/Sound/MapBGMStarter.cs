using UnityEngine;

public class MapBGMStarter : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Inst.PlayMapBGM();
    }
}