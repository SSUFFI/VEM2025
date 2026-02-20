using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrder : MonoBehaviour
{
    [Header("Renderer Groups")]
    [SerializeField] Renderer[] backRenderers;     // 맨 뒤 (캐릭터 일러)
    [SerializeField] Renderer[] midRenderers;      // 중간 (카드 테두리)
    [SerializeField] Renderer[] frontRenderers;    // 맨 위 (텍스트, 공격력, 체력 UI)

    [SerializeField] string sortingLayerName;

    int originOrder;

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }

    public void SetOrder(int order)
    {
        // 카드 하나당 30단위 확보 (3그룹 × 10단계)
        int baseOrder = order * 30;

        // 맨 뒤 그룹
        for (int i = 0; i < backRenderers.Length; i++)
        {
            backRenderers[i].sortingLayerName = sortingLayerName;
            backRenderers[i].sortingOrder = baseOrder + i; // 0~?
        }

        // 중간 그룹
        for (int i = 0; i < midRenderers.Length; i++)
        {
            midRenderers[i].sortingLayerName = sortingLayerName;
            midRenderers[i].sortingOrder = baseOrder + 10 + i;
        }

        // 맨 위 그룹
        for (int i = 0; i < frontRenderers.Length; i++)
        {
            frontRenderers[i].sortingLayerName = sortingLayerName;
            frontRenderers[i].sortingOrder = baseOrder + 20 + i;
        }
    }
}
