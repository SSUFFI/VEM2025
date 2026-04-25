using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NodeState
{
    Locked,
    Available,
    Cleared,
    Current
}

public class NodeUI : MonoBehaviour
{
    [Header("Node Info")]
    public int nodeID;
    public int laneIndex;
    public List<int> nextNodesID = new List<int>();

    [Header("Special")]
    public bool isGoalNode;

    [Header("UI")]
    public Button button;

    [Header("Base")]
    [SerializeField] Image baseImage;
    [SerializeField] Sprite baseNormal;
    [SerializeField] Sprite baseGray;

    [Header("Icon")]
    [SerializeField] Image iconImage;

    [Header("Data")]
    public NodeDataSO nodeData;

    [HideInInspector] public NodeState state;

    float pulseTimer;
    Vector3 baseScale;
    bool isSelected = false;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (isSelected)
        {
            pulseTimer += Time.deltaTime * 2f;

            float t = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;
            float scale = 1f + t * 0.05f;

            transform.localScale = baseScale * scale;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }

    public void SetState(NodeState newState)
    {
        state = newState;

        baseImage.color = Color.white;
        iconImage.color = Color.white;

        switch (newState)
        {
            case NodeState.Locked:
                button.interactable = false;
                baseImage.sprite = baseGray;

                if (nodeData != null)
                {
                    if (nodeData.nodeType == NodeType.Event || isGoalNode)
                    {
                        iconImage.enabled = true;
                        iconImage.sprite = nodeData.iconGray;
                    }
                    else
                    {
                        iconImage.enabled = false;
                    }
                }
                break;

            case NodeState.Available:
                button.interactable = true;
                baseImage.sprite = baseNormal;

                if (nodeData != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = nodeData.iconNormal;
                }
                break;

            case NodeState.Cleared:
                button.interactable = false;
                baseImage.sprite = baseGray;

                if (nodeData != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = nodeData.iconGray;
                }

                baseImage.color = new Color(0.35f, 0.35f, 0.35f, 1f);
                iconImage.color = new Color(0.35f, 0.35f, 0.35f, 1f);
                break;

            case NodeState.Current:
                button.interactable = false;
                baseImage.sprite = baseNormal;

                if (nodeData != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = nodeData.iconNormal;
                }
                break;
        }

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;

        if (isSelected)
            transform.SetAsLastSibling();
    }
}