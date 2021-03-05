using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool isGameActive = false;

    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    public static event Action OnQuicksave;
    public static event Action OnLoadQuickSave;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;

    }

    public void Quicksave()
    {
        OnQuicksave?.Invoke();
    }

    public void LoadQuickSave()
    {
        OnLoadQuickSave?.Invoke();
    }


    public void ToggleGame()
    {
        if (isGameActive)
            EndGame();
        else
            StartGame();
    }
    public void StartGame()
    {
        OTime.time = 0;
        isGameActive = true;

        OnGameStart?.Invoke();

        Quicksave();
    }

    public void EndGame()
    {
        isGameActive = false;

        OnGameEnd?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
