using UnityEngine;

public class BattleBackgroundManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer backgroundRenderer;

    [Header("Tutorial")]
    [SerializeField] Sprite tutorialBackground;

    void Start()
    {
        SetupBackground();
    }

    void SetupBackground()
    {
        if (backgroundRenderer == null)
            return;

        if (BattleData.IsBattleTutorial)
        {
            if (tutorialBackground != null)
                backgroundRenderer.sprite = tutorialBackground;

            return;
        }

        if (BattleData.selectedEnemyDeck != null &&
            BattleData.selectedEnemyDeck.battleBackground != null)
        {
            backgroundRenderer.sprite =
                BattleData.selectedEnemyDeck.battleBackground;
        }
    }
}