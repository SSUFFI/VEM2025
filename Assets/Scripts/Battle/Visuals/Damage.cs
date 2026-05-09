using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damage : MonoBehaviour
{
    [SerializeField] TMP_Text damageTMP;

    Transform tr;

    Vector3 baseScale;

    static Dictionary<Transform, int> hitStackCount =
        new Dictionary<Transform, int>();

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void SetupTransform(Transform tr)
    {
        this.tr = tr;
    }

    public void Damaged(int damage)
    {
        if (damage <= 0)
            return;

        GetComponent<CardOrder>().SetOrder(1000);

        damageTMP.text = $"-{damage}";

        int stack = 0;

        if (tr != null)
        {
            if (hitStackCount.ContainsKey(tr))
            {
                hitStackCount[tr]++;
                stack = hitStackCount[tr];
            }
            else
            {
                hitStackCount[tr] = 0;
            }

            Vector3 offset = Vector3.zero;

            float scaleMultiplier = 1f;

            if (stack > 0)
            {
                offset = new Vector3(
                    Random.Range(-0.8f, 0.8f),
                    Random.Range(0.3f, 1.0f),
                    0);

                scaleMultiplier = 0.8f;
            }

            transform.position =
                tr.position + offset;

            transform.localScale =
                baseScale * scaleMultiplier;
        }
    }

    public void DestroySelf()
    {
        if (tr != null &&
            hitStackCount.ContainsKey(tr))
        {
            hitStackCount[tr]--;

            if (hitStackCount[tr] <= 0)
                hitStackCount.Remove(tr);
        }

        Destroy(gameObject);
    }
}