using UnityEngine;

public static class StageProgress
{
    public static int selectedStage = 1;

    public static int highestClearedStage
    {
        get
        {
            return PlayerPrefs.GetInt(
                AccountManager.GetAccountKey(
                    "HighestClearedStage"),
                0);
        }

        set
        {
            string key =
                AccountManager.GetAccountKey(
                    "HighestClearedStage");

            PlayerPrefs.SetInt(
                key,
                value);

            PlayerPrefs.Save();
        }
    }

    public static void ClearStage(int stage)
    {
        if (stage <= highestClearedStage)
            return;

        highestClearedStage = stage;

        Debug.Log(
            $"스테이지 진행도 저장 : {stage}");
    }
}