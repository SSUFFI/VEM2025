using System.Collections.Generic;
using UnityEngine;

public static class NodeMapRuntimeData
{
    public static bool initialized = false;

    public static HashSet<int> clearedNodeIDs = new HashSet<int>();

    public static int currentNodeID = -1;
    public static int blockedLaneMax = -1;

    public static Dictionary<int, NodeDataSO> savedNodeData
        = new Dictionary<int, NodeDataSO>();

    public static bool hasExtraLife = false;

    public static void ObtainExtraLife()
    {
        hasExtraLife = true;
    }

    public static bool ConsumeExtraLife()
    {
        if (!hasExtraLife)
            return false;

        hasExtraLife = false;
        return true;
    }

    public static void ResetRun()
    {
        initialized = false;
        clearedNodeIDs.Clear();
        currentNodeID = -1;
        blockedLaneMax = -1;
        savedNodeData.Clear();

        hasExtraLife = false;
    }
}