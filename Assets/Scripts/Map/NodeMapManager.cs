using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StageData
{
    public string stageName;

    [Header("Start")]
    public NodeDataSO startData;

    [Header("Battle Tiers")]
    public List<NodeDataSO> tier1List = new List<NodeDataSO>();
    public List<NodeDataSO> tier2List = new List<NodeDataSO>();
    public List<NodeDataSO> tier3List = new List<NodeDataSO>();

    [Header("Events")]
    public NodeDataSO event_NPC;
    public NodeDataSO event_Heal;
    public NodeDataSO event_Explore;
    public NodeDataSO event_Tablet;

    [Header("Boss")]
    public NodeDataSO bossData;
}

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

    [Header("Stage Data")]
    [SerializeField] private List<StageData> stageDataList = new List<StageData>();

    [Header("Event Panels")]
    [SerializeField] GameObject pnl_Event_NPC;
    [SerializeField] GameObject pnl_Event_Heal;
    [SerializeField] GameObject pnl_Event_Explore;
    [SerializeField] GameObject pnl_Event_Tablet;

    private StageData currentStageData;

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
                if (pnl_Event_NPC != null) pnl_Event_NPC.SetActive(true);
                break;
            case EventType.Heal:
                if (pnl_Event_Heal != null) pnl_Event_Heal.SetActive(true);
                break;
            case EventType.Explore:
                if (pnl_Event_Explore != null) pnl_Event_Explore.SetActive(true);
                break;
            case EventType.Tablet:
                if (pnl_Event_Tablet != null) pnl_Event_Tablet.SetActive(true);
                break;
        }
    }

    public void CloseAllEventPanels()
    {
        if (pnl_Event_NPC != null) pnl_Event_NPC.SetActive(false);
        if (pnl_Event_Heal != null) pnl_Event_Heal.SetActive(false);
        if (pnl_Event_Explore != null) pnl_Event_Explore.SetActive(false);
        if (pnl_Event_Tablet != null) pnl_Event_Tablet.SetActive(false);
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

        // if (bottomButtonsRoot != null)
        //     bottomButtonsRoot.SetActive(false);
    }

    public void InitializeMap()
    {
        selectedNodeID = -1;

        SetCurrentStageData();

        if (!NodeMapRuntimeData.initialized)
        {
            clearedNodeIDs.Clear();
            currentNodeID = -1;
            blockedLaneMax = -1;

            GenerateNodeData();

            NodeMapRuntimeData.savedNodeData.Clear();

            foreach (NodeUI node in allNodes)
                NodeMapRuntimeData.savedNodeData[node.nodeID] = node.nodeData;

            NodeMapRuntimeData.initialized = true;
            NodeMapRuntimeData.clearedNodeIDs = new HashSet<int>();
            NodeMapRuntimeData.currentNodeID = -1;
            NodeMapRuntimeData.blockedLaneMax = -1;
        }
        else
        {
            clearedNodeIDs = new HashSet<int>(NodeMapRuntimeData.clearedNodeIDs);
            currentNodeID = NodeMapRuntimeData.currentNodeID;
            blockedLaneMax = NodeMapRuntimeData.blockedLaneMax;

            foreach (NodeUI node in allNodes)
            {
                if (NodeMapRuntimeData.savedNodeData.ContainsKey(node.nodeID))
                    node.nodeData = NodeMapRuntimeData.savedNodeData[node.nodeID];
            }
        }

        foreach (NodeUI node in allNodes)
        {
            node.SetState(NodeState.Locked);
            node.SetSelected(false);
        }

        RefreshAvailableNodes();
    }

    void SetCurrentStageData()
    {
        if (stageDataList == null || stageDataList.Count == 0)
        {
            Debug.LogError("NodeMapManager: stageDataList가 비어있습니다. Inspector에서 1막/2막/3막 데이터를 넣어주세요.");
            currentStageData = null;
            return;
        }

        int stageIndex = Mathf.Clamp(StageProgress.selectedStage - 1, 0, stageDataList.Count - 1);
        currentStageData = stageDataList[stageIndex];
    }

    public void OnClickNode(int nodeID)
    {
        if (!nodeDict.ContainsKey(nodeID))
            return;

        NodeUI clickedNode = nodeDict[nodeID];

        if (clickedNode.state != NodeState.Available)
            return;

        if (clickedNode.nodeData != null &&
            clickedNode.nodeData.nodeType == NodeType.Start)
        {
            selectedNodeID = nodeID;
            ClearSelectedNode();
            return;
        }

        selectedNodeID = nodeID;

        foreach (NodeUI node in allNodes)
            node.SetSelected(node.nodeID == selectedNodeID);

        if (NodeInfoPanelUI.Inst != null)
            NodeInfoPanelUI.Inst.Show(clickedNode.nodeData);
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

        if (node.nodeData == null)
        {
            Debug.LogWarning("선택한 노드에 nodeData가 없습니다.");
            return;
        }

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
            BattleData.selectedEnemyDeck = node.nodeData.enemyDeck;
            BattleData.selectedNodeType = node.nodeData.nodeType;
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

        if (NodeInfoPanelUI.Inst != null)
            NodeInfoPanelUI.Inst.Hide();

        RefreshAvailableNodes();

        NodeMapRuntimeData.clearedNodeIDs = new HashSet<int>(clearedNodeIDs);
        NodeMapRuntimeData.currentNodeID = currentNodeID;
        NodeMapRuntimeData.blockedLaneMax = blockedLaneMax;
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
        if (currentStageData == null)
            return;

        var startNode = allNodes.Find(n => n.nodeID == startNodeID);
        if (startNode != null)
            startNode.nodeData = currentStageData.startData;

        List<NodeUI> availableNodes = new List<NodeUI>(allNodes);

        availableNodes.RemoveAll(n => n.nodeID == startNodeID);

        var lane2Node = availableNodes.Find(n => n.laneIndex == 1);
        if (lane2Node != null)
        {
            lane2Node.nodeData = GetRandomTier1();
            availableNodes.Remove(lane2Node);
        }

        var lane10Node = availableNodes.Find(n => n.laneIndex == 9);
        if (lane10Node != null)
        {
            lane10Node.nodeData = currentStageData.event_Tablet;
            availableNodes.Remove(lane10Node);
        }

        var lane11Node = availableNodes.Find(n => n.laneIndex == 10);
        if (lane11Node != null)
        {
            lane11Node.nodeData = currentStageData.bossData;
            availableNodes.Remove(lane11Node);
        }

        var tier3Candidates = availableNodes.FindAll(n => n.laneIndex >= 6 && n.laneIndex <= 8);

        for (int i = 0; i < 2; i++)
        {
            if (tier3Candidates.Count == 0)
                break;

            var node = GetRandomNode(tier3Candidates);
            if (node == null)
                break;

            node.nodeData = GetRandomTier3();
            availableNodes.Remove(node);
            tier3Candidates.Remove(node);
        }

        List<NodeDataSO> eventList = new List<NodeDataSO>()
        {
            currentStageData.event_NPC,
            currentStageData.event_Heal,
            currentStageData.event_Explore
        };

        foreach (var e in eventList)
        {
            if (e == null) continue;
            if (availableNodes.Count == 0) break;

            var node = GetRandomNode(availableNodes);
            if (node == null)
                break;

            node.nodeData = e;
            availableNodes.Remove(node);
        }

        var tier2Candidates = availableNodes.FindAll(n => n.laneIndex >= 3 && n.laneIndex <= 8);

        for (int i = 0; i < 3; i++)
        {
            if (tier2Candidates.Count == 0)
                break;

            var node = GetRandomNode(tier2Candidates);
            if (node == null)
                break;

            node.nodeData = GetRandomTier2();
            availableNodes.Remove(node);
            tier2Candidates.Remove(node);
        }

        foreach (var node in availableNodes)
        {
            node.nodeData = GetRandomTier1();
        }
    }

    NodeUI GetRandomNode(List<NodeUI> list)
    {
        if (list == null || list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }

    NodeDataSO GetRandomTier1()
    {
        if (currentStageData == null || currentStageData.tier1List == null || currentStageData.tier1List.Count == 0)
        {
            Debug.LogError("NodeMapManager: 현재 Stage의 Tier1 List가 비어있습니다.");
            return null;
        }

        return currentStageData.tier1List[Random.Range(0, currentStageData.tier1List.Count)];
    }

    NodeDataSO GetRandomTier2()
    {
        if (currentStageData == null || currentStageData.tier2List == null || currentStageData.tier2List.Count == 0)
        {
            Debug.LogError("NodeMapManager: 현재 Stage의 Tier2 List가 비어있습니다.");
            return GetRandomTier1();
        }

        return currentStageData.tier2List[Random.Range(0, currentStageData.tier2List.Count)];
    }

    NodeDataSO GetRandomTier3()
    {
        if (currentStageData == null || currentStageData.tier3List == null || currentStageData.tier3List.Count == 0)
        {
            Debug.LogError("NodeMapManager: 현재 Stage의 Tier3 List가 비어있습니다.");
            return GetRandomTier2();
        }

        return currentStageData.tier3List[Random.Range(0, currentStageData.tier3List.Count)];
    }
}