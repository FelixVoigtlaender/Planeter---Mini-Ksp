using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GravitySystem))]
public class GravitySystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GravitySystem gravitySytem = (GravitySystem)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Check System"))
        {
            gravitySytem.CheckSystem();
        }


        //gravitySytem.CheckSystem();

        GUILayout.EndHorizontal();
    }

}
