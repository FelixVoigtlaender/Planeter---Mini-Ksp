using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Stage currentStage;
    public ContentCopy contentCopy;

    public GameObject[] startStages;

    private void Start()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameEnd += OnGameEnd;
    }

    public void OnGameStart()
    {

    }
    public void OnGameEnd()
    {
        contentCopy.ClearContent();
        contentCopy.FillContent();
    }

    public Stage GetCurrentStage()
    {
        if (IsStagesEmpty())
            return null;
        if (currentStage && currentStage.isActiveAndEnabled)
            return currentStage;

        Transform lastChild = transform.GetChild(transform.childCount - 1);
        currentStage = lastChild.GetComponent<Stage>();
        return currentStage;
    }
    public bool IsStagesEmpty()
    {
        return transform.childCount == 0;
    }
}
