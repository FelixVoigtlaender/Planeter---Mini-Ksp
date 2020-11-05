using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameActive = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartGame()
    {
        isGameActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
