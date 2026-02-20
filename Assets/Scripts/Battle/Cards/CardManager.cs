using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }

    void Awake()
    {
        Inst = this;

        if (myCards == null) myCards = new List<Card>();

        if (otherCards == null) otherCards = new List<Card>();

        if (myDeckSO == null)
            myDeckSO = Resources.Load<DeckSO>("DeckSO/MyDeckSO");

        if (enemyDeckSO == null)
            enemyDeckSO = Resources.Load<DeckSO>("DeckSO/EnemyDeckSO");

        Debug.Log("MyDeckSO Loaded: " + (myDeckSO != null));
        Debug.Log("EnemyDeckSO Loaded: " + (enemyDeckSO != null));
    }

    [Header("Deck Data")]
    [SerializeField] DeckSO myDeckSO;
    [SerializeField] DeckSO enemyDeckSO;
    [SerializeField] TMP_Text myDeckCountTMP;
    [SerializeField] TMP_Text enemyDeckCountTMP;
    List<CardData> myDeck;
    List<CardData> enemyDeck;

    [Header("Card Prefab & Positions")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<Card> myCards;
    [SerializeField] List<Card> otherCards;
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform otherCardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;
    [SerializeField] GameObject handZoomPanel;
    [SerializeField] RectTransform zoomContent;
    [SerializeField] Transform zoomRoot;

    enum ECardState { Nothing, CanMouseOver, CanMouseDrag }
    [SerializeField] ECardState eCardState;

    Card selectCard;
    bool isMyCardDrag;
    bool onMyCardArea;
    bool cardClickedThisFrame = false;
    public bool isZoomMode = false;
    const int MAX_HAND = 10;

    // ---------------------------- 덱 셔플 세팅 ----------------------------

    void Start()
    {
        SetupDecks();

        TurnManager.OnAddCard += AddCard;
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;
    }

    void SetupDecks()
    {
        // 덱 생성
        myDeck = new List<CardData>(myDeckSO.deckItems);
        enemyDeck = new List<CardData>(enemyDeckSO.deckItems);

        Shuffle(myDeck);
        Shuffle(enemyDeck);

        UpdateDeckCountUI();
    }

    void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    public void UpdateDeckCountUI()
    {
        if (myDeckCountTMP != null)
            myDeckCountTMP.text = myDeck.Count.ToString();

        if (enemyDeckCountTMP != null)
            enemyDeckCountTMP.text = enemyDeck.Count.ToString();
    }

    // ---------------------------- 덱에서 카드 1장 뽑기 ----------------------------

    public CardData PopItem(bool isMine)
    {
        var deck = isMine ? myDeck : enemyDeck;

        if (deck.Count == 0)
            return null;

        CardData data = deck[0];
        deck.RemoveAt(0);
        return data;
    }

    // ---------------------------- 턴 관리 ----------------------------

    void Update()
    {
        if (isZoomMode)
        {
            // 이번 프레임에 마우스 왼쪽 버튼 눌림
            if (Input.GetMouseButtonDown(0))
            {
                // 카드가 클릭된 프레임이 아니면 → 배경 클릭으로 보고 줌 종료
                if (!cardClickedThisFrame)
                {
                    ExitZoomMode();
                }

                // 다음 프레임을 위해 플래그 리셋
                cardClickedThisFrame = false;
            }

            // 줌 모드에선 아래 로직 막기
            return;
        }

        // 줌 모드가 아닐 때는 항상 플래그 초기화
        cardClickedThisFrame = false;

        // 원래 드래그 / 카드 영역 감지 / 상태 설정
        if (isMyCardDrag)
            CardDrag();

        DetectCardArea();
        SetECardState();
    }


    // ---------------------------- 카드 드로우 ----------------------------

    void AddCard(bool isMine)
    {
        var hand = isMine ? myCards : otherCards;
        if (hand.Count >= MAX_HAND)
        {
            if (isMine)
                BattleGameManager.Inst.Notification("드로우 불가 (최대 10장)");
            return;
        }

        var data = PopItem(isMine);
        if (data == null)
            return;

        Vector3 startPos = isMine || otherCardSpawnPoint == null
            ? cardSpawnPoint.position
            : otherCardSpawnPoint.position;

        var cardObject = Instantiate(cardPrefab, startPos, Utils.QI);
        var card = cardObject.GetComponent<Card>();

        card.Setup(data, isMine);
        (isMine ? myCards : otherCards).Add(card);

        SetOriginOrder(isMine);
        CardAlignment(isMine);

        UpdateDeckCountUI();
    }

    void SetOriginOrder(bool isMine)
    {
        int count = isMine ? myCards.Count : otherCards.Count;
        for (int i = 0; i < count; i++)
        {
            var targetCard = isMine ? myCards[i] : otherCards[i];
            targetCard?.GetComponent<CardOrder>().SetOriginOrder(i);
        }
    }

    // ---------------------------- 손패 정렬 ----------------------------

    void CardAlignment(bool isMine)
    {
        if (isZoomMode)
            return;

        List<PRS> originPRSList = new List<PRS>();

        if (isMine)
            originPRSList = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 0.3f);
        else
            originPRSList = RoundAlignment(otherCardLeft, otherCardRight, otherCards.Count, -0.5f, Vector3.one * 0.3f);

        var targetCards = isMine ? myCards : otherCards;

        for (int i = 0; i < targetCards.Count; i++)
        {
            targetCards[i].originPRS = originPRSList[i];
            targetCards[i].MoveTransform(targetCards[i].originPRS, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int count, float height, Vector3 scale)
    {
        float[] lerps = new float[count];
        List<PRS> result = new List<PRS>(count);

        switch (count)
        {
            case 1: lerps = new float[] { 0.5f }; break;
            case 2: lerps = new float[] { 0.27f, 0.73f }; break;
            case 3: lerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (count - 1);
                for (int i = 0; i < count; i++)
                    lerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < count; i++)
        {
            var pos = Vector3.Lerp(leftTr.position, rightTr.position, lerps[i]);
            var rot = Quaternion.identity;

            if (count >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(lerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                pos.y += curve;
                rot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, lerps[i]);
            }

            result.Add(new PRS(pos, rot, scale));
        }

        return result;
    }

    public void OnCardClicked(Card card)
    {
        cardClickedThisFrame = true;

        if (!isZoomMode)
        {
            EnterZoomMode();
            return;
        }

        StartDragFromZoom(card);
    }
    // ---------------------------- 손패 확대 ---------------------------------

    public void StartDragFromZoom(Card card)
    {
        isZoomMode = false;

        selectCard = card;
        isMyCardDrag = true;

        CardDrag();
    }

    public void EnterZoomMode()
    {
        if (isZoomMode || myCards.Count == 0)
            return;

        isZoomMode = true;


        float zoomScaleValue = 0.4f;
        Vector3 zoomScale = Vector3.one * zoomScaleValue;


        float cardWidth = myCards[0].GetComponent<SpriteRenderer>().bounds.size.x * zoomScaleValue;
        float spacing = cardWidth * 2f;

        int count = myCards.Count;
        float startX = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var card = myCards[i];
            float posX = startX + spacing * i;

            Vector3 worldPos = zoomRoot.position + new Vector3(posX, 0f, 0f);

            card.MoveTransform(new PRS(worldPos, Quaternion.identity, zoomScale), true, 0.25f);
            card.GetComponent<CardOrder>().SetMostFrontOrder(true);
        }
    }

    public void OnZoomBackgroundClick()
    {
        ExitZoomMode();
    }

    public void ExitZoomMode()
    {
        if (!isZoomMode)
           return;

        isZoomMode = false;

        for (int i = 0; i < myCards.Count; i++)
        {
            var card = myCards[i];
            card.GetComponent<CardOrder>().SetMostFrontOrder(false);
            card.MoveTransform(card.originPRS, true, 0.25f);
        }

        if (handZoomPanel != null)
            handZoomPanel.SetActive(false);
    }


    public void DamageDeck(int amount, bool isMine, Entity deckAttacker = null)
    {
        var deck = isMine ? myDeck : enemyDeck;

        amount = Mathf.Min(amount, deck.Count);

        for (int i = 0; i < amount; i++)
        {
            var cardItem = deck[0];
            deck.RemoveAt(0);

            GraveManager.Inst.AddToGraveFromDeck(cardItem, isMine, deckAttacker);

        }

        UpdateDeckCountUI();
    }

    void ReturnSelectedCardToHand()
    {
        if (selectCard == null) return;

        EntityManager.Inst.RemoveMyEmptyEntity();

        selectCard.transform.DOKill();
        selectCard.MoveTransform(selectCard.originPRS, true, 0.25f);

        selectCard.GetComponent<CardOrder>().SetMostFrontOrder(false);
        CardAlignment(true);
    }

    public bool TryPutCard(bool isMine)
    {
        if (!isMine && otherCards.Count <= 0)
            return false;

        Card card = isMine ? selectCard : otherCards[Random.Range(0, otherCards.Count)];
        var spawnPos = isMine ? Utils.MousePos : card.transform.position;

        var targetCards = isMine ? myCards : otherCards;

        int cost = card.data.manaCost;

        if (!TurnManager.Inst.CanPayMana(isMine, cost))
        {
            if (isMine) BattleGameManager.Inst.Notification("마나가 부족합니다");
            return false;
        }

        if (EntityManager.Inst.SpawnEntity(isMine, card.data, spawnPos))
        {
            TurnManager.Inst.PayMana(isMine, cost);

            targetCards.Remove(card);
            card.transform.DOKill();
            Destroy(card.gameObject);

            if (isMine)
            {
                selectCard = null;
            }

            CardAlignment(isMine);
            return true;
        }
        else
        {
            targetCards.ForEach(x => x.GetComponent<CardOrder>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
    }

    // ---------------------------- 마우스 & 드래그 ----------------------------

    public void CardMouseOver(Card card)
    {
        return;
    }

    public void CardMouseExit(Card card)
    {
        return;
    }

    public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        isMyCardDrag = true;
    }

    public void CardMouseUp()
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onMyCardArea)
        {
            EntityManager.Inst.RemoveMyEmptyEntity();
            return;
        }

        bool success = TryPutCard(true);

        if (!success)
        {
            ReturnSelectedCardToHand();
        }
    }

    void CardDrag()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);
        }
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    void EnlargeCard(bool isEnlarge, Card card)
    {
        return;
    }

    void SetECardState()
    {
        if (TurnManager.Inst.isLoading)
            eCardState = ECardState.Nothing;

        else if (!TurnManager.Inst.myTurn || EntityManager.Inst.IsFullMyEntities)
            eCardState = ECardState.CanMouseOver;

        else if (TurnManager.Inst.myTurn)
            eCardState = ECardState.CanMouseDrag;
    }
}
