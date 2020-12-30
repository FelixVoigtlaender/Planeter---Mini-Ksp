using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OrbitBodyGenerator : MonoBehaviour
{
    [Header("Scaling")]
    public float scaleSemiMajorAxis;
    public float scaleMass;
    public float scaleRadius;
    [Header("Generation")]
    public GameObject planetPrefab;
    public OrbitElements orbitElements;
    string path;
    string jsonString;
    public void LoadOrbitElements()
    {
        // File Setup
        path = Application.streamingAssetsPath + "/Planets.json";
        jsonString = File.ReadAllText(path);

        // Orbit Elements
        orbitElements = OrbitElements.CreateFromJSON(jsonString);
        orbitElements.ApplyScale(scaleSemiMajorAxis, scaleMass, scaleRadius);
    }

    public void GeneratePlanets()
    {
        if (orbitElements == null || orbitElements.planets == null)
            return;

        // Generate Planets
        OrbitElement[] planetElements = orbitElements.planets;
        List<GravitySystem> existingSystems = new List<GravitySystem>( FindObjectsOfType<GravitySystem>());
        for (int i = 0; i < planetElements.Length; i++)
        {
            GravitySystem system = FindGravitySystem(planetElements[i].name, existingSystems.ToArray());

            if(system == null)
            {
                GameObject planet = Instantiate(planetPrefab);
                system = planet.GetComponent<GravitySystem>();
                existingSystems.Add(system);
            }

            system.orbitElement = planetElements[i];
            system.gameObject.name = planetElements[i].name;
        }

        // Setup Planets
        foreach(GravitySystem gs in existingSystems)
        {
            gs.Setup(gs.orbitElement);
        }

    }

    GravitySystem FindGravitySystem(string name, GravitySystem[] systems)
    {
        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i].orbitElement.name == name)
                return systems[i];
        }
        return null;
    }
}
