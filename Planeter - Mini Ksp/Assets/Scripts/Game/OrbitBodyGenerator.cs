using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OrbitBodyGenerator : MonoBehaviour
{
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static OrbitBodyGenerator s_Instance = null;
    // A static property that finds or creates an instance of the manager object and returns it.
    public static OrbitBodyGenerator instance
    {
        get
        {
            if (s_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(OrbitBodyGenerator)) as OrbitBodyGenerator;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                var obj = new GameObject("Manager");
                s_Instance = obj.AddComponent<OrbitBodyGenerator>();
            }

            return s_Instance;
        }
    }


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

    public string[] GetPlanetNames()
    {
        string[] planetNames = new string[0];
        if (orbitElements == null || orbitElements.planets == null)
            return planetNames;

        planetNames = new string[orbitElements.planets.Length];
        for (int i = 0; i < orbitElements.planets.Length; i++)
        {
            planetNames[i] = orbitElements.planets[i].name;
        }


        return planetNames;
    }

    public OrbitElement GetPlanet(string name)
    {
        if (orbitElements == null || orbitElements.planets == null)
            return null;

        for (int i = 0; i < orbitElements.planets.Length; i++)
        {
            if (orbitElements.planets[i].name.Equals(name.Trim()))
                return orbitElements.planets[i];
        }

        return null;
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
