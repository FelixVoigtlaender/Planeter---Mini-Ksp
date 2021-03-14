using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StageManager : MonoBehaviour
{
    public Stage currentStage;


    public event Action onStagesChanged;


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
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
        }
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
        return ActiveStageCount() == 0;
    }

    public int ActiveStageCount()
    {
        int count = 0;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
                count++;

        }
        return count;
    }
    public int TotalStageCount()
    {
        return transform.childCount;
    }

    public Vector2 Ignite(Vector2 dir)
    {
        if (!GameManager.isGameActive)
            return Vector2.zero;
        if (IsStagesEmpty())
            return Vector2.zero;

        Vector2 thrust = GetCurrentStage().Ignite(dir);

        onStagesChanged?.Invoke();
        return thrust;
    }
}
