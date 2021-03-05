using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitElements
{
    public OrbitElement[] planets;
    public OrbitElement[] moons;
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
        foreach (OrbitElement element in moons)
        {
            element.ApplyScale(scaleSemiMajorAxis, scaleMass, scaleRadius);
        }
    }

    public void ApplyScale(OrbitBodyGenerator.Scaling planetScales, OrbitBodyGenerator.Scaling moonScales )
    {
        foreach (OrbitElement element in planets)
        {
            element.ApplyScale(planetScales.scaleSemiMajorAxis, planetScales.scaleMass, planetScales.scaleRadius);
        }
        foreach (OrbitElement element in moons)
        {
            element.ApplyScale(moonScales.scaleSemiMajorAxis,moonScales.scaleMass,planetScales.scaleRadius);
        }
    }
}
