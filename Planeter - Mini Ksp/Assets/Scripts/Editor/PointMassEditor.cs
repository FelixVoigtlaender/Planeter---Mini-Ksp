using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointMass))]
public class PointMassEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PointMass pointMass = (PointMass)target;

        

    }
}
