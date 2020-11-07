using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameText : MonoBehaviour
{
    [TextArea]
    public string inGameText = "";
    [TextArea]
    public string outGameText = "";
    public TextMeshProUGUI textField;

    public void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();

        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameEnd += OnGameEnd;
    }

    public void OnGameStart()
    {
        SetText(inGameText);
    }
    public void OnGameEnd()
    {
        SetText(outGameText);
    }
    public void SetText(string text)
    {
        textField.SetText(text);
    }


}
