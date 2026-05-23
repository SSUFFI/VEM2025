using UnityEngine;
using TMPro;

public class Heal : MonoBehaviour
{
    [SerializeField] TMP_Text healTMP;
    [SerializeField] float destroyTime = 1.5f;

    Transform tr;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    public void SetupTransform(Transform tr)
    {
        this.tr = tr;
    }

    void Update()
    {
        if (tr != null)
            transform.position = tr.position;
    }

    public void Healed(int heal)
    {
        if (heal <= 0)
            return;

        GetComponent<CardOrder>().SetOrder(1000);

        healTMP.text = heal.ToString();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}