using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrder : MonoBehaviour
{
    [Header("Renderer Groups")]
    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] midRenderers;
    [SerializeField] Renderer[] frontRenderers;

    [SerializeField] string sortingLayerName;

    int originOrder;

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        if (isMostFront)
        {
            SetOrder(500);
        }
        else
        {
            SetOrder(originOrder);
        }
    }

    public void SetOrder(int order)
    {
        int baseOrder = order * 30;

        for (int i = 0; i < backRenderers.Length; i++)
        {
            backRenderers[i].sortingLayerName = sortingLayerName;
            backRenderers[i].sortingOrder = baseOrder + i;
        }

        for (int i = 0; i < midRenderers.Length; i++)
        {
            midRenderers[i].sortingLayerName = sortingLayerName;
            midRenderers[i].sortingOrder = baseOrder + 10 + i;
        }

        for (int i = 0; i < frontRenderers.Length; i++)
        {
            frontRenderers[i].sortingLayerName = sortingLayerName;
            frontRenderers[i].sortingOrder = baseOrder + 20 + i;
        }
    }
}
