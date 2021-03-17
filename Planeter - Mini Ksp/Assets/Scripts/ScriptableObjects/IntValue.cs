using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName ="Data/IntValue")]
[Serializable]
public class IntValue : ScriptableObject
{

    [SerializeField] int value;
    public event Action OnValueChanged;
    public int Value 
    { 
        get => this.value; 
        set { this.value = value; OnValueChanged?.Invoke(); } 
    }
}
