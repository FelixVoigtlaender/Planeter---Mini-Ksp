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


        if (GUILayout.Button("Load OrbitElements"))
        {
            generator.LoadOrbitElements();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Planets"))
        {
            generator.GeneratePlanets();
        }
        if (GUILayout.Button("Generate Moons"))
        {
            generator.GenerateMoons();
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Do all"))
        {
            generator.LoadOrbitElements();
            generator.GeneratePlanets();
            generator.GenerateMoons();
        }
        if (GUILayout.Button("CheckSystem"))
        {
            generator.CheckSystem();
        }
        GUILayout.EndHorizontal();
    }

}