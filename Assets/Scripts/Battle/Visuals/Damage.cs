using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damage : MonoBehaviour
{
    [SerializeField] TMP_Text damageTMP;
    Transform tr;

    public void SetupTransform(Transform tr)
    {
        this.tr = tr;
    }

    void Update()
    {
        if (tr != null)
            transform.position = tr.position;
    }

    public void Damaged(int damage)
    {
        if (damage <= 0)
            return;

        GetComponent<CardOrder>().SetOrder(1000);
        damageTMP.text = $"-{damage}";

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}