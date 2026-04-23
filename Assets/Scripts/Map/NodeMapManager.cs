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

    [SerializeField] NodeDataSO startData;

    [SerializeField] List<NodeDataSO> stage1List;
    [SerializeField] List<NodeDataSO> stage2List;
    [SerializeField] List<NodeDataSO> stage3List;

    [SerializeField] NodeDataSO event_NPC;
    [SerializeField] NodeDataSO event_Heal;
    [SerializeField] NodeDataSO event_Explore;
    [SerializeField] NodeDataSO event_Tablet;

    [SerializeField] NodeDataSO bossData;

    [SerializeField] GameObject pnl_Event_NPC;
    [SerializeField] GameObject pnl_Event_Heal;
    [SerializeField] GameObject pnl_Event_Explore;
    [SerializeField] GameObject pnl_Event_Tablet;

    private Dictionary<int, NodeUI> nodeDict = new Dictionary<int, NodeUI>();
    private HashSet<int> clearedNodeIDs = new HashSet<int>();

    private int currentNodeID = -1;
    private int selectedNodeID = -1;
    private int blockedLaneMax = -1;

    public void OpenEventPanel(EventType type)
    {
        CloseAllEventPanels();

        switch (type)
        {
            case EventType.NPC:
                pnl_Event_NPC.SetActive(true);
                break;
            case EventType.Heal:
                pnl_Event_Heal.SetActive(true);
                break;
            case EventType.Explore:
                pnl_Event_Explore.SetActive(true);
                break;
            case EventType.Tablet:
                pnl_Event_Tablet.SetActive(true);
                break;
        }
    }

    public void CloseAllEventPanels()
    {
        pnl_Event_NPC.SetActive(false);
        pnl_Event_Heal.SetActive(false);
        pnl_Event_Explore.SetActive(false);
        pnl_Event_Tablet.SetActive(false);
    }

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

        LoadClearedNode();

        if (stagePanel != null)
            stagePanel.SetActive(true);

        if (bottomButtonsRoot != null)
            bottomButtonsRoot.SetActive(false);
    }


    public void InitializeMap()
    {
        clearedNodeIDs.Clear();
        currentNodeID = -1;
        selectedNodeID = -1;
        blockedLaneMax = -1;

        GenerateNodeData();

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

    void LoadClearedNode()
    {
        int clearedID = PlayerPrefs.GetInt("ClearedNodeID", -1);

        if (clearedID == -1)
            return;

        selectedNodeID = clearedID;

        ClearSelectedNode();

        PlayerPrefs.DeleteKey("ClearedNodeID");
    }

    public void OnClickStartGame()
    {
        if (selectedNodeID == -1)
            return;

        NodeUI node = nodeDict[selectedNodeID];

        if (node.nodeData.nodeType == NodeType.Start)
        {
            ClearSelectedNode();
            return;
        }

        if (node.nodeData.nodeType == NodeType.Event)
        {
            OpenEventPanel(node.nodeData.eventType);
            return;
        }

        if (node.nodeData != null)
        {
            if (node.nodeData.enemyDeck != null)
            {
                BattleData.selectedEnemyDeck = node.nodeData.enemyDeck;
            }
        }

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

    public void ClearSelectedNode()
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

    void GenerateNodeData()
    {
        var startNode = allNodes.Find(n => n.nodeID == startNodeID);
        startNode.nodeData = startData;

        List<NodeUI> availableNodes = new List<NodeUI>(allNodes);

        availableNodes.RemoveAll(n => n.nodeID == startNodeID);

        var lane2Node = availableNodes.Find(n => n.laneIndex == 1);
        lane2Node.nodeData = GetRandomStage1();
        availableNodes.Remove(lane2Node);

        var lane10Node = availableNodes.Find(n => n.laneIndex == 9);
        lane10Node.nodeData = event_Tablet;
        availableNodes.Remove(lane10Node);

        var lane11Node = availableNodes.Find(n => n.laneIndex == 10);
        lane11Node.nodeData = bossData;
        availableNodes.Remove(lane11Node);

        var tier3Candidates = availableNodes.FindAll(n => n.laneIndex >= 6 && n.laneIndex <= 8);

        for (int i = 0; i < 2; i++)
        {
            var node = GetRandomNode(tier3Candidates);
            node.nodeData = GetRandomStage3();
            availableNodes.Remove(node);
            tier3Candidates.Remove(node);
        }

        List<NodeDataSO> eventList = new List<NodeDataSO>()
    {
        event_NPC,
        event_Heal,
        event_Explore
    };

        foreach (var e in eventList)
        {
            var node = GetRandomNode(availableNodes);
            node.nodeData = e;
            availableNodes.Remove(node);
        }

        var tier2Candidates = availableNodes.FindAll(n => n.laneIndex >= 3 && n.laneIndex <= 8);

        for (int i = 0; i < 3; i++)
        {
            var node = GetRandomNode(tier2Candidates);
            node.nodeData = GetRandomStage2();
            availableNodes.Remove(node);
            tier2Candidates.Remove(node);
        }

        foreach (var node in availableNodes)
        {
            node.nodeData = GetRandomStage1();
        }
    }

    NodeUI GetRandomNode(List<NodeUI> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    NodeDataSO GetRandomStage1()
    {
        return stage1List[Random.Range(0, stage1List.Count)];
    }

    NodeDataSO GetRandomStage2()
    {
        return stage2List[Random.Range(0, stage2List.Count)];
    }

    NodeDataSO GetRandomStage3()
    {
        return stage3List[Random.Range(0, stage3List.Count)];
    }
}