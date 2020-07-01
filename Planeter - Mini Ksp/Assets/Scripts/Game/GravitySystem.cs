using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class GravitySystem : PointMass
{
    public SpriteRenderer renderer;

    public GameObject systemPrefab;
    public PointMass mainMass;
    public float radiusOfInfluence = 0;

    public float totalMass;

    List<PointMass> masses = new List<PointMass>();

    public void CheckSystem()
    {
        //Radius of Influence
        GravitySystem parentSystem = null;

        if (!renderer)
            renderer = GetComponent<SpriteRenderer>();
        
        if (transform.parent)
        {
            parentSystem = transform.parent.GetComponent<GravitySystem>();
        }

        totalMass = GetTotalMass();
        if (parentSystem)
        {
            float distToParentSytem = ((Vector2)parentSystem.transform.position - (Vector2)transform.position).magnitude;
            radiusOfInfluence = OrbitMath.CircleOfInfluence(distToParentSytem, totalMass, parentSystem.totalMass);


        }
        else
        {
            radiusOfInfluence = 10000;
        }

        //Add System in other Children
        if (parentSystem)
        {
            GravitySystem[] siblingSystems = parentSystem.GetChildSystems();
            foreach(GravitySystem siblingSystem in siblingSystems)
            {
                if (siblingSystem == this)
                    continue;

                float distance = Vector2.Distance(transform.position, siblingSystem.transform.position);
                if(distance < radiusOfInfluence && siblingSystem.GetMainMass() < GetMainMass())
                {
                    siblingSystem.transform.SetParent(transform);
                }
                if(distance < siblingSystem.radiusOfInfluence && siblingSystem.GetMainMass() > GetMainMass())
                {
                    transform.SetParent(siblingSystem.transform);
                }
            }
        }

        // Check if Exited System
        if (parentSystem)
        {

            float distance = Vector2.Distance(transform.position, parentSystem.transform.position);
            if(distance > parentSystem.radiusOfInfluence)
            {
                transform.SetParent(parentSystem.transform.parent);
            }
        }

    }

    public float GetMainMass()
    {
        mainMass = mainMass ? mainMass : GetComponent<PointMass>();
        return mainMass.GetMass();
    }
    
    public float GetTotalMass()
    {
        float totalMass = 0;
        
        // Mass of Object itself
        mainMass = GetComponent<PointMass>();
        if (mainMass)
            totalMass += mainMass.GetMass();

        // Mass of children
        totalMass += GetTotalChildMass();

        return totalMass;
    }

    public float GetTotalChildMass()
    {
        float totalMass = 0;
        GravitySystem[] childSystems = GetChildSystems();
        foreach (GravitySystem childSystem in childSystems)
        {
            totalMass += childSystem.GetTotalMass();
        }
        return totalMass;
    }


    public void AddSystem(GravitySystem system)
    {
        system.transform.SetParent(transform);
    }

    public void CheckChildSystems()
    {
        GravitySystem[] childSystems = GetChildSystems();
        foreach (GravitySystem childSystem in childSystems)
        {
            childSystem.CheckSystem();
        }
    }

    public GravitySystem[] GetChildSystems()
    {
        List<GravitySystem> childSystems = new List<GravitySystem>();
        foreach(Transform child in transform)
        {
            GravitySystem childSystem = child.GetComponent<GravitySystem>();
            if (childSystem)
                childSystems.Add(childSystem);
        }
        return childSystems.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        if (renderer)
            Gizmos.color = renderer.color;
        Gizmos.DrawWireSphere(transform.position, radiusOfInfluence);

        foreach(Transform child in transform)
        {
            Gizmos.DrawLine(transform.position, child.position);
        }
        
        if (transform.parent)
        {
            Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) - radiusOfInfluence);
            Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) + radiusOfInfluence);
        }
    }

}
