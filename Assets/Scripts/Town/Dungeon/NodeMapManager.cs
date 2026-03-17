using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NodeMapManager : MonoBehaviour
{
    public static NodeMapManager Inst { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject stagePanel;

    [Header("Nodes")]
    [SerializeField] private List<NodeUI> allNodes = new List<NodeUI>();
    [SerializeField] private int startNodeID = 0;
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Bottom UI")]
    [SerializeField] private GameObject bottomButtonsRoot;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button clearStageTempButton;

    private Dictionary<int, NodeUI> nodeDict = new Dictionary<int, NodeUI>();
    private HashSet<int> clearedNodeIDs = new HashSet<int>();

    private int currentNodeID = -1;
    private int selectedNodeID = -1;
    private int blockedLaneMax = -1;

    private void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(gameObject);

        nodeDict.Clear();

        foreach (NodeUI node in allNodes)
        {
            if (node != null && !nodeDict.ContainsKey(node.nodeID))
                nodeDict.Add(node.nodeID, node);
        }
    }

    private void Start()
    {
        InitializeMap();

        if (stagePanel != null)
            stagePanel.SetActive(false);

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(false);
    }

    public void OpenStagePanel()
    {
        if (stagePanel != null)
            stagePanel.SetActive(true);
    }

    public void CloseStagePanel()
    {
        if (stagePanel != null)
            stagePanel.SetActive(false);

        selectedNodeID = -1;

        foreach (NodeUI node in allNodes)
            node.SetSelected(false);

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(false);
    }

    public void InitializeMap()
    {
        clearedNodeIDs.Clear();
        currentNodeID = -1;
        selectedNodeID = -1;
        blockedLaneMax = -1;

        foreach (NodeUI node in allNodes)
        {
            node.SetState(NodeState.Locked);
            node.SetSelected(false);
        }

        if (nodeDict.ContainsKey(startNodeID))
            nodeDict[startNodeID].SetState(NodeState.Available);

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(false);
    }

    public void OnClickNode(int nodeID)
    {
        if (!nodeDict.ContainsKey(nodeID))
            return;

        NodeUI clickedNode = nodeDict[nodeID];

        if (clickedNode.state != NodeState.Available)
            return;

        selectedNodeID = nodeID;

        foreach (NodeUI node in allNodes)
            node.SetSelected(node.nodeID == selectedNodeID);

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(true);
    }

    public void OnClickStartGame()
    {
        if (selectedNodeID == -1)
            return;

        PlayerPrefs.SetInt("SelectedNodeID", selectedNodeID);
        PlayerPrefs.Save();

        SceneManager.LoadScene(battleSceneName);
    }

    public void OnClickClearStageTemp()
    {
        if (selectedNodeID == -1)
            return;

        ClearSelectedNode();
    }

    private void ClearSelectedNode()
    {
        if (!nodeDict.ContainsKey(selectedNodeID))
            return;

        NodeUI selectedNode = nodeDict[selectedNodeID];

        clearedNodeIDs.Add(selectedNodeID);

        if (selectedNode.laneIndex > blockedLaneMax)
            blockedLaneMax = selectedNode.laneIndex;

        currentNodeID = selectedNodeID;

        foreach (NodeUI node in allNodes)
            node.SetSelected(false);

        selectedNodeID = -1;

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(false);

        RefreshAvailableNodes();
    }

    private void RefreshAvailableNodes()
    {
        foreach (NodeUI node in allNodes)
        {
            if (clearedNodeIDs.Contains(node.nodeID))
                node.SetState(NodeState.Cleared);
            else
                node.SetState(NodeState.Locked);
        }

        if (currentNodeID == -1)
        {
            if (nodeDict.ContainsKey(startNodeID))
                nodeDict[startNodeID].SetState(NodeState.Available);

            return;
        }

        NodeUI currentNode = nodeDict[currentNodeID];

        foreach (int nextID in currentNode.nextNodesID)
        {
            if (!nodeDict.ContainsKey(nextID))
                continue;

            NodeUI nextNode = nodeDict[nextID];

            if (clearedNodeIDs.Contains(nextID))
                continue;

            if (nextNode.laneIndex <= blockedLaneMax)
                continue;

            nextNode.SetState(NodeState.Available);
        }
    }
}