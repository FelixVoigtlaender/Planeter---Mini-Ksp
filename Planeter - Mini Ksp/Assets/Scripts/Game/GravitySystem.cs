using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class GravitySystem : PointMass
{
    [Header("My Mass")]
    public float radiusOfInfluence = 0;
    public GravitySystem parentSystem;

    public Vector2 localStartPosition;

    public void Awake()
    {
        localStartPosition = transform.localPosition;
        if(transform.parent)
            parentSystem = transform.parent.GetComponent<GravitySystem>();
    }


    public void FixedUpdate()
    {
        OrbitMath.OrbitPrediction prediction = OrbitMath.GetStaticOrbitPrediction(Time.time, this);
        transform.localPosition = prediction.localPosition;


    }

    public void CheckSystem()
    {

        if (!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();

        //Radius of Influence
        GravitySystem parentSystem = null;
        if (transform.parent)
        {
            parentSystem = transform.parent.GetComponent<GravitySystem>();
        }
        if (parentSystem)
        {
            float distToParentSytem = ((Vector2)parentSystem.transform.position - (Vector2)transform.position).magnitude;
            radiusOfInfluence = OrbitMath.CircleOfInfluence(distToParentSytem, GetMass(), parentSystem.GetMass());
            
        }
        else
        {
            radiusOfInfluence = 10000;
        }

        // Add Siblings in RadiusOfInfluence
        if (parentSystem)
        {
            GravitySystem[] siblingSystems = parentSystem.GetChildSystems();
            foreach(GravitySystem siblingSystem in siblingSystems)
            {
                if (siblingSystem == this)
                    continue;

                float distance = Vector2.Distance(transform.position, siblingSystem.transform.position);
                if(distance < radiusOfInfluence && siblingSystem.GetMass() < GetMass())
                {
                    siblingSystem.transform.SetParent(transform);
                }
                if(distance < siblingSystem.radiusOfInfluence && siblingSystem.GetMass() > GetMass())
                {
                    transform.SetParent(siblingSystem.transform);
                }
            }
        }

        // Check if this system exited another System
        if (parentSystem)
        {

            float distance = Vector2.Distance(transform.position, parentSystem.transform.position);
            if(distance > parentSystem.radiusOfInfluence)
            {
                transform.SetParent(parentSystem.transform.parent);
            }
        }

    }


    /// <summary>
    /// Adds total mass together
    /// </summary>
    /// <returns> mass of all masses in system </returns>
    public override float GetMass()
    {
        mass = 0;

        mass += base.GetMass();

        PointMass[] systemMasses = transform.GetComponentsInChildren<PointMass>();
        foreach(PointMass pm in systemMasses)
        {
            if (pm.gameObject.GetInstanceID() == GetInstanceID() || pm == this)
                continue;

            mass += pm.GetMass();
        }
        return mass;
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

    public GravitySystem GetParentSystem()
    {
        if (!transform.parent)
            return null;

        return transform.parent.GetComponent<GravitySystem>();
    }
    
    private void OnDrawGizmos()
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
