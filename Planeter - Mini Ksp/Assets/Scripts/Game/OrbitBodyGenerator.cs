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

    [Header("System")]
    public GravitySystem mainSystem;
    [Header("Scaling")]
    public Scaling planetScales;
    public Scaling moonScales;
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

        planetScales = ApplyConversion(planetScales);
        moonScales = ApplyConversion(moonScales);
        orbitElements.ApplyScale(planetScales,moonScales);
    }

    public Scaling ApplyConversion(Scaling scaling)
    {
        switch (scaling.conversionType)
        {
            case ConversionType.AU:
                scaling.scaleRadius = scaling.scaleSemiMajorAxis * (OMath.Er() / OMath.Au());
                break;
            case ConversionType.RADIUS:
                scaling.scaleSemiMajorAxis = scaling.scaleRadius * (OMath.Au() / OMath.Er());
                break;
        }
        return scaling;
    }

    public string[] GetElementNames()
    {
        var myList = new List<string>();
        myList.AddRange(GetPlanetNames());
        myList.AddRange(GetMoonNames());
        return myList.ToArray();
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
    public string[] GetMoonNames()
    {
        string[] moonNames = new string[0];
        if (orbitElements == null || orbitElements.moons == null)
            return moonNames;

        moonNames = new string[orbitElements.moons.Length];
        for (int i = 0; i < orbitElements.moons.Length; i++)
        {
            moonNames[i] = orbitElements.moons[i].name;
        }


        return moonNames;
    }
    public OrbitElement GetElement(string name)
    {
        OrbitElement element = GetPlanet(name);
        if (element == null)
            element = GetMoon(name);
        return element;
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

    public OrbitElement GetMoon(string name)
    {
        if (orbitElements == null || orbitElements.moons == null)
            return null;

        for (int i = 0; i < orbitElements.moons.Length; i++)
        {
            if (orbitElements.moons[i].name.Equals(name.Trim()))
                return orbitElements.moons[i];
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
    public void GenerateMoons()
    {
        if (orbitElements == null || orbitElements.moons == null)
            return;

        // Generate Planets
        OrbitElement[] moonElements = orbitElements.moons;
        List<GravitySystem> existingSystems = new List<GravitySystem>(FindObjectsOfType<GravitySystem>());
        for (int i = 0; i < moonElements.Length; i++)
        {
            GravitySystem system = FindGravitySystem(moonElements[i].name, existingSystems.ToArray());

            if (system == null)
            {
                GameObject moon = Instantiate(planetPrefab);
                system = moon.GetComponent<GravitySystem>();
                existingSystems.Add(system);
            }

            system.orbitElement = moonElements[i];
            system.gameObject.name = moonElements[i].name;
        }

        // Setup Planets
        foreach (GravitySystem gs in existingSystems)
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

    public void CheckSystem()
    {
        if (!mainSystem)
            return;
        mainSystem.CheckSystem();
    }

    [System.Serializable]
    public struct Scaling
    {
        public float scaleSemiMajorAxis;
        public float scaleMass;
        public float scaleRadius;
        public ConversionType conversionType;
    }

    [System.Serializable]
    public enum ConversionType{
        RADIUS,AU,NONE
    }
}
