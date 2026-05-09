using UnityEngine;

public class PlayerRelicManager : MonoBehaviour
{
    public static PlayerRelicManager Inst;

    public RelicDataSO equippedRelic;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EquipRelic(RelicDataSO relic)
    {
        equippedRelic = relic;
    }
}