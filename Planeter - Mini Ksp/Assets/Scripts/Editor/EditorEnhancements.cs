using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorEnhancements : MonoBehaviour
{
}

[CustomEditor(typeof(OrbitBodyGenerator))]
public class OrbitBodyGeneratroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OrbitBodyGenerator generator = (OrbitBodyGenerator)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load OrbitElements"))
        {
            generator.LoadOrbitElements();
        }
        if (GUILayout.Button("Generate Planets"))
        {
            generator.GeneratePlanets();
        }

        GUILayout.EndHorizontal();
    }

}