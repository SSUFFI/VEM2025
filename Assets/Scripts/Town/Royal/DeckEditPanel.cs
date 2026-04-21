using System.Collections.Generic;
using UnityEngine;

public class DeckEditPanel : MonoBehaviour
{
    public Transform cardListParent;
    public GameObject cardListUIPrefab;

    void Start()
    {
        GenerateCardList();
    }

    void GenerateCardList()
    {
        foreach (Transform child in cardListParent)
            Destroy(child.gameObject);

        var cards = CardPool.Inst.ownedCards;

        foreach (var data in cards)
        {
            var go = Instantiate(cardListUIPrefab, cardListParent);
            go.GetComponent<CardListUI>().Init(data);
        }
    }
}