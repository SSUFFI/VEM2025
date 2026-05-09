using UnityEngine;

public class PileVisual : MonoBehaviour
{
    public enum PileType
    {
        Deck,
        Grave
    }

    [Header("Type")]
    [SerializeField] PileType pileType;

    [Header("Owner")]
    [SerializeField] bool isMine;

    [Header("Renderer")]
    [SerializeField] SpriteRenderer pileRenderer;

    [Header("Sprites")]
    [SerializeField] Sprite spriteA;
    [SerializeField] Sprite spriteB;
    [SerializeField] Sprite spriteC;
    [SerializeField] Sprite spriteD;

    [Header("Deck")]
    [SerializeField] int maxDeckCount = 40;

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        int count = GetCurrentCount();

        switch (pileType)
        {
            case PileType.Deck:
                RefreshDeck(count);
                break;

            case PileType.Grave:
                RefreshGrave(count);
                break;
        }
    }

    int GetCurrentCount()
    {
        switch (pileType)
        {
            case PileType.Deck:

                if (CardManager.Inst == null)
                    return 0;

                return isMine
                    ? CardManager.Inst.MyDeckCount
                    : CardManager.Inst.EnemyDeckCount;

            case PileType.Grave:

                if (GraveManager.Inst == null)
                    return 0;

                return isMine
                    ? GraveManager.Inst.MyGraveCount
                    : GraveManager.Inst.EnemyGraveCount;
        }

        return 0;
    }

    void RefreshDeck(int count)
    {
        if (count <= 0)
        {
            pileRenderer.sprite = null;
            return;
        }

        float percent =
            (float)count / maxDeckCount;

        if (percent > 0.5f)
        {
            pileRenderer.sprite = spriteA;
        }
        else if (percent > 0.2f)
        {
            pileRenderer.sprite = spriteB;
        }
        else if (percent > 0.05f)
        {
            pileRenderer.sprite = spriteC;
        }
        else
        {
            pileRenderer.sprite = spriteD;
        }
    }

    void RefreshGrave(int count)
    {
        if (count <= 0)
        {
            pileRenderer.sprite = null;
        }
        else if (count <= 3)
        {
            pileRenderer.sprite = spriteD;
        }
        else if (count <= 10)
        {
            pileRenderer.sprite = spriteC;
        }
        else if (count <= 20)
        {
            pileRenderer.sprite = spriteB;
        }
        else
        {
            pileRenderer.sprite = spriteA;
        }
    }
}