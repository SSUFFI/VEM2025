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
    public Image iconImage;

    public GameObject currentMark;
    public GameObject clearMark;
    public GameObject lockMark;
    public GameObject selectedMark;

    [HideInInspector] public NodeState state;

    float pulseTimer;

    void Update()
    {
        if (state == NodeState.Available)
        {
            pulseTimer += Time.deltaTime * 2f;
            float scale = 1f + Mathf.Sin(pulseTimer) * 0.05f;
            transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public void SetState(NodeState newState)
    {
        state = newState;

        if (currentMark != null) currentMark.SetActive(false);
        if (clearMark != null) clearMark.SetActive(false);
        if (lockMark != null) lockMark.SetActive(false);

        switch (newState)
        {
            case NodeState.Locked:
                button.interactable = false;

                if (isGoalNode)
                {
                    // Goal은 잠겨 있어도 회색으로 보이기
                    iconImage.enabled = true;
                    iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else
                {
                    // 일반 잠긴 노드는 숨김
                    iconImage.enabled = false;
                }
                break;

            case NodeState.Available:
                button.interactable = true;
                iconImage.enabled = true;
                iconImage.color = Color.white;
                break;

            case NodeState.Cleared:
                button.interactable = false;
                iconImage.enabled = true;
                iconImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);

                if (clearMark != null)
                    clearMark.SetActive(true);
                break;

            case NodeState.Current:
                button.interactable = false;
                iconImage.enabled = true;
                iconImage.color = Color.white;

                if (currentMark != null)
                    currentMark.SetActive(true);
                break;
        }

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedMark != null)
            selectedMark.SetActive(isSelected);
    }
}