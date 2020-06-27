using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<MonoBehaviour> disableInGame = new List<MonoBehaviour>();


    public void StartGame()
    {
        foreach(MonoBehaviour behaviour in disableInGame)
        {
            behaviour.enabled = false;
        }
    }
    public void EndGame()
    {
        foreach(MonoBehaviour behaviour in disableInGame)
        {
            behaviour.enabled = true;
        }
    }
}
