using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitElements
{
    public OrbitElement[] planets;
    public static OrbitElements CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<OrbitElements>(jsonString);
    }

    public void ApplyScale(float scaleSemiMajorAxis, float scaleMass, float scaleRadius)
    {
        foreach(OrbitElement element in planets)
        {
            element.ApplyScale(scaleSemiMajorAxis, scaleMass, scaleRadius);
        }
    }
}
