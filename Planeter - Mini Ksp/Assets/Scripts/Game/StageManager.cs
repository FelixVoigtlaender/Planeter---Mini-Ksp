using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Stage currentStage;
    public ContentCopy contentCopy;

    private void Start()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameEnd += OnGameEnd;
        GameManager.OnQuicksave += OnQuickSave;
        GameManager.OnLoadQuickSave += OnLoadQuickSave;
    }

    public void OnGameStart()
    {

    }
    public void OnGameEnd()
    {
        contentCopy.ClearContent();
        contentCopy.FillContent();
    }

    public void OnQuickSave()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.GetComponent<Stage>().OnQuickSave();

        }
    }
    public void OnLoadQuickSave()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.GetComponent<Stage>().OnLoadQuickSave();
        }
    }
    public Stage GetCurrentStage()
    {
        if (IsStagesEmpty())
            return null;
        if (currentStage && currentStage.isActiveAndEnabled)
            return currentStage;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
                return child.GetComponent<Stage>();

        }
        return null;
    }
    public bool IsStagesEmpty()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
                return false;

        }
        return true;
    }
}
