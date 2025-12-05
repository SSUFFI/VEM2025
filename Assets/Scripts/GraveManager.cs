using System.Collections.Generic;
using UnityEngine;

public class GraveManager : MonoBehaviour
{
    public static GraveManager Inst;

    public List<Item> myGrave = new List<Item>();
    public List<Item> enemyGrave = new List<Item>();

    void Awake()
    {
        Inst = this;
    }

    public void AddToGrave(Item item, bool isMine)
    {
        if (item == null) return;

        if (isMine)
            myGrave.Add(item); 
        else
            enemyGrave.Add(item);
    }
}
