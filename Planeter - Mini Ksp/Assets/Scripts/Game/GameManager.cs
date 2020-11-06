﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool isGameActive = false;
    public PageSwiper pageSwiper;

    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
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
        isGameActive = true;
        pageSwiper.enabled = false;
        OTime.time = 0;

        OnGameStart?.Invoke();
    }

    public void EndGame()
    {
        isGameActive = false;
        pageSwiper.enabled = true;

        OnGameEnd?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
