using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActivation : MonoBehaviour
{
    public bool inGame = true;
    public void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameEnd += OnGameEnd;
    }

    public void OnGameStart()
    {
        gameObject.SetActive(inGame);
    }
    public void OnGameEnd()
    {
        gameObject.SetActive(!inGame);
    }
}
