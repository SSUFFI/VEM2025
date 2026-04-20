using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleButton : MonoBehaviour
{
    public void OnClickTemple()
    {
        if (TutorialManager.Inst != null &&
            TutorialManager.Inst.step != TutorialStep.Done)
        {
            TutorialManager.Inst.OnTempleEntered();
            return;
        }

        TempleUIManager.Inst.OpenMainPanel();
    }
}