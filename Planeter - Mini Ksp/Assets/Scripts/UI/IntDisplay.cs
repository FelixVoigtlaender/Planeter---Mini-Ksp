using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntDisplay : MonoBehaviour
{
    public IntValue firstValue;
    public IntValue secondValue;
    public TextMeshProUGUI text;
    public string prefix;
    public string midfix;
    public string suffix;

    private void Start()
    {
        if (!text)
            text = GetComponent<TextMeshProUGUI>();

        if(firstValue != null)
            firstValue.OnValueChanged += OnValueChanged;
        if(secondValue != null)
            secondValue.OnValueChanged += OnValueChanged;
    }

    public void OnValueChanged()
    {
        if (!text)
            text = GetComponent<TextMeshProUGUI>();


        string s = "";
        s += prefix;
        s += firstValue != null ? firstValue.Value.ToString() : "";
        s += midfix;
        s += secondValue != null ? secondValue.Value.ToString() : "";
        s += suffix;
        text.text = s;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        OnValueChanged();
    }
#endif

}
