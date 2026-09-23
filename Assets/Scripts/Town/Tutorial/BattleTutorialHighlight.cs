using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleTutorialHighlight : MonoBehaviour
{
    public static BattleTutorialHighlight Inst;

    const float DIM_VALUE = 0.38f;

    readonly Dictionary<SpriteRenderer, Color>
        spriteOriginalColors =
            new Dictionary<SpriteRenderer, Color>();

    readonly Dictionary<Graphic, Color>
        graphicOriginalColors =
            new Dictionary<Graphic, Color>();

    readonly Dictionary<TMP_Text, Color>
        textOriginalColors =
            new Dictionary<TMP_Text, Color>();

    bool isDimmed;

    void Awake()
    {
        Inst = this;
    }

    public void DimAll()
    {
        if (isDimmed)
            return;

        isDimmed = true;

        SpriteRenderer[] renderers =
            FindObjectsOfType<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            SaveAndDim(renderer);
        }

        Graphic[] graphics =
            FindObjectsOfType<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            // TMP는 아래에서 따로 처리
            if (graphic is TMP_Text)
                continue;

            SaveAndDim(graphic);
        }

        TMP_Text[] texts =
            FindObjectsOfType<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            SaveAndDim(text);
        }
    }

    public void Highlight(GameObject target)
    {
        if (!isDimmed || target == null)
            return;

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            if (spriteOriginalColors.TryGetValue(renderer, out Color original))
            {
                renderer.color = original;
            }
            else
            {
                Color current = renderer.color;

                Color restored = new Color(
                    Mathf.Clamp01(current.r / DIM_VALUE),
                    Mathf.Clamp01(current.g / DIM_VALUE),
                    Mathf.Clamp01(current.b / DIM_VALUE),
                    current.a
                );

                spriteOriginalColors.Add(renderer, restored);
                renderer.color = restored;
            }
        }

        Graphic[] graphics =
            target.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            if (graphic is TMP_Text)
                continue;

            if (graphicOriginalColors.TryGetValue(graphic, out Color original))
            {
                graphic.color = original;
            }
            else
            {
                Color current = graphic.color;

                Color restored = new Color(
                    Mathf.Clamp01(current.r / DIM_VALUE),
                    Mathf.Clamp01(current.g / DIM_VALUE),
                    Mathf.Clamp01(current.b / DIM_VALUE),
                    current.a
                );

                graphicOriginalColors.Add(graphic, restored);
                graphic.color = restored;
            }
        }

        TMP_Text[] texts =
            target.GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            if (textOriginalColors.TryGetValue(text, out Color original))
            {
                text.color = original;
            }
            else
            {
                Color current = text.color;

                Color restored = new Color(
                    Mathf.Clamp01(current.r / DIM_VALUE),
                    Mathf.Clamp01(current.g / DIM_VALUE),
                    Mathf.Clamp01(current.b / DIM_VALUE),
                    current.a
                );

                textOriginalColors.Add(text, restored);
                text.color = restored;
            }
        }
    }

    public void DimTarget(GameObject target)
    {
        if (!isDimmed || target == null)
            return;

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(
                true);

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            if (spriteOriginalColors.TryGetValue(
                renderer,
                out Color original))
            {
                renderer.color =
                    GetDimColor(original);
            }
        }

        Graphic[] graphics =
            target.GetComponentsInChildren<Graphic>(
                true);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            if (graphic is TMP_Text)
                continue;

            if (graphicOriginalColors.TryGetValue(
                graphic,
                out Color original))
            {
                graphic.color =
                    GetDimColor(original);
            }
        }

        TMP_Text[] texts =
            target.GetComponentsInChildren<TMP_Text>(
                true);

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            if (textOriginalColors.TryGetValue(
                text,
                out Color original))
            {
                text.color =
                    GetDimColor(original);
            }
        }
    }

    public void DimNewTarget(GameObject target)
    {
        if (!isDimmed || target == null)
            return;

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(
                true);

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            SaveAndDim(renderer);
        }

        Graphic[] graphics =
            target.GetComponentsInChildren<Graphic>(
                true);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            if (graphic is TMP_Text)
                continue;

            SaveAndDim(graphic);
        }

        TMP_Text[] texts =
            target.GetComponentsInChildren<TMP_Text>(
                true);

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            SaveAndDim(text);
        }
    }

    void SaveAndDim(SpriteRenderer renderer)
    {
        if (!spriteOriginalColors.ContainsKey(renderer))
        {
            spriteOriginalColors.Add(
                renderer,
                renderer.color);
        }

        renderer.color =
            GetDimColor(
                spriteOriginalColors[renderer]);
    }

    void SaveAndDim(Graphic graphic)
    {
        if (!graphicOriginalColors.ContainsKey(graphic))
        {
            graphicOriginalColors.Add(
                graphic,
                graphic.color);
        }

        graphic.color =
            GetDimColor(
                graphicOriginalColors[graphic]);
    }

    void SaveAndDim(TMP_Text text)
    {
        if (!textOriginalColors.ContainsKey(text))
        {
            textOriginalColors.Add(
                text,
                text.color);
        }

        text.color =
            GetDimColor(
                textOriginalColors[text]);
    }

    Color GetDimColor(Color original)
    {
        return new Color(
            original.r * DIM_VALUE,
            original.g * DIM_VALUE,
            original.b * DIM_VALUE,
            original.a);
    }

    public void Clear()
    {
        foreach (var pair in spriteOriginalColors)
        {
            if (pair.Key != null)
                pair.Key.color = pair.Value;
        }

        foreach (var pair in graphicOriginalColors)
        {
            if (pair.Key != null)
                pair.Key.color = pair.Value;
        }

        foreach (var pair in textOriginalColors)
        {
            if (pair.Key != null)
                pair.Key.color = pair.Value;
        }

        spriteOriginalColors.Clear();
        graphicOriginalColors.Clear();
        textOriginalColors.Clear();

        isDimmed = false;
    }

    void OnDestroy()
    {
        if (isDimmed)
            Clear();

        if (Inst == this)
            Inst = null;
    }
}