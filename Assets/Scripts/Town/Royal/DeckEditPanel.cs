using System.Collections.Generic;
using UnityEngine;

public class DeckEditPanel : MonoBehaviour
{
    public Transform cardListParent;
    public GameObject cardListUIPrefab;

    public List<CardDataSO> allCards;

    void Start()
    {
        GenerateCardList();
    }

    void GenerateCardList()
    {
        foreach (var data in allCards)
        {
            var go = Instantiate(cardListUIPrefab, cardListParent);
            go.GetComponent<CardListUI>().Init(data);
        }
    }
}