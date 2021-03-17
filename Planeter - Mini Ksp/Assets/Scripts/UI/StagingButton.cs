using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagingButton : MonoBehaviour
{
    public StageManager stageManager;
    public StageSetup stageSetup;

    private void Start()
    {
        stageSetup.totalPoints.OnValueChanged += CheckButton;
        GameManager.OnGameEnd += CheckButton;
        GameManager.OnGameStart += CheckButton;
        stageManager.onStagesChanged += CheckButton;
    }

    public void PointsChanged(int n)
    {
        CheckButton();
    }

    public void CheckButton()
    {
        bool notPlayed = stageManager.ActiveStageCount() == stageManager.TotalStageCount();
        bool noStagesLeft = stageManager.IsStagesEmpty();
        bool noPoints = stageSetup.totalPoints.Value <= 3;
        bool notIngame = !GameManager.isGameActive;

        bool isActive = (notPlayed || noStagesLeft || notIngame) && !noPoints;

        gameObject.SetActive(isActive);
    }
}
