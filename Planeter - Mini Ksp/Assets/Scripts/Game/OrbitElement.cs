using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitElement
{
    public string name = "OrbitElement";
    public string center = "Sun";
    public float a_semiMajorAxis;
    public float e_eccentricity;
    public float mass;
    public float radius;
    public static OrbitElement CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<OrbitElement>(jsonString);
    }

    public void ApplyScale(float scaleSemiMajorAxis, float scaleMass, float scaleRadius)
    {
        a_semiMajorAxis *= scaleSemiMajorAxis;
        mass *= scaleMass;
        radius *= scaleRadius;
    }
}
