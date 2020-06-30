using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem : PointMass
{
    GameManager SystemPrefab;

    List<PointMass> masses = new List<PointMass>();

    public void SetUpSystem()
    {
    }

    public Vector2 TotalGravityForce(Vector2 position, float mass = 1)
    {
        Vector2 totalForce = Vector2.zero;
        int count = 0;
        foreach (PointMass pointMass in masses)
        {
            if (((Vector2)pointMass.transform.position - position).magnitude < 0.1)
                continue;


            count++;
            totalForce += pointMass.GravityForce(position,mass);
        }
        totalForce /= count;
        return totalForce;
    }
    public void AddMass(float mass)
    {
        this.mass += mass;
    }
    //
    // System
    //
    public void AddGravitySystem(GravitySystem gravitySystem)
    {
        if (gravitySystem == this)
            return;

        masses.Add(gravitySystem);
        gravitySystem.transform.parent = transform;


        AddMass(gravitySystem.mass);
    }
    public void AddPointMasses(PointMass[] pointMasses)
    {
        foreach(PointMass pm in pointMasses)
        {
            AddPointMass(pm);
        }
    }
    public void AddPointMass(PointMass pointMass)
    {
        masses.Add(pointMass);
        pointMass.transform.parent = transform;


        AddMass(pointMass.mass);
    }
}
