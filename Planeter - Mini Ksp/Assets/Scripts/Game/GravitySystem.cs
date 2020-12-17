﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Linq;

public class GravitySystem : PointMass
{
    public static GravitySystem sunSystem;
    [Header("My Mass")]
    public float radiusOfInfluence = 0;
    public GravitySystem parentSystem;
    public SpriteRenderer sphereOfInfluence;

    public Vector2 localStartPosition;
    public float t0 = 0;

    public OrbitElements orbitElements = new OrbitElements();

    //In Game
    public GravitySystem[] childSystems;

    public void Awake()
    {
        if (transform.parent)
            parentSystem = transform.parent.GetComponent<GravitySystem>();
        else
            sunSystem = this;

        //Collect ChildSystems
        List<GravitySystem> systems = new List<GravitySystem>();
        foreach (Transform child in transform)
        {
            GravitySystem system = child.GetComponent<GravitySystem>();
            if (system)
                systems.Add(system);
        }
        childSystems = systems.ToArray();


        //Orbit Prediction Setup
        localStartPosition = transform.localPosition;
        t0 = OrbitMath.GetT0(this);

        //Draw Orbit
        CheckOrbit();
    }


    public void Start()
    {
    }


    public void FixedUpdate()
    {
        OrbitMath.OrbitPrediction prediction = OrbitMath.GetStaticOrbitPrediction(OTime.time, this,true);
        transform.localPosition = prediction.localPosition;
    }

    public OrbitMath.OrbitPrediction DynamicPrediction(OrbitMath.OrbitPrediction prediction, float mass = 1)
    {
        //Set Prediction to this type
        prediction.ChangeSystem(this);

        //Outside of system
        if (prediction.localPosition.magnitude > radiusOfInfluence && parentSystem)
            return parentSystem.DynamicPrediction(prediction, mass);

        //Inside of child system
        foreach(GravitySystem childSystem in childSystems)
        {
            if(childSystem.IsInSystem(prediction.time, prediction.localPosition))
            {
                return childSystem.DynamicPrediction(prediction, mass);
            }
        }

        //Calculate Gravity
        prediction.localGravity = TotalGravity(prediction.time, prediction.localPosition, mass);

        return prediction;
    }

    // LocalPosition to parent system
    public bool IsInSystem(float time, Vector2 parentLocalPosition)
    {
        Vector2 myLocalPosition = GetPrediction(time).localPosition;
        float distance = (parentLocalPosition - myLocalPosition).magnitude;
        return distance < radiusOfInfluence;
    }

    /// <summary>
    /// Returns the Center Gravity till sun system
    /// </summary>
    /// <param name="time"></param>
    /// <param name="localPosition"></param>
    /// <param name="mass"></param>
    /// <returns></returns>
    public Vector2 ParentGravity(float time, Vector2 localPosition, float mass = 1)
    {
        if (!parentSystem)
            return Vector2.zero;

        Vector2 myLocalPosition = GetPrediction(time).localPosition;
        Vector2 parentLocalPosition = localPosition + myLocalPosition;
        float distance = parentLocalPosition.magnitude;
        float parentMass = parentSystem.GetCenterMass();
        Vector2 gravity = OrbitMath.GravityForce(parentLocalPosition,Vector2.zero,mass,parentMass);


        return gravity + parentSystem.ParentGravity(time, parentLocalPosition, mass);
    }
    

    public Vector2 TotalGravity(float time, Vector2 localPosition, float mass=1)
    {
        Vector2 gravityVector = Vector2.zero;
        // Parent Mass
        //Vector2 parentGravity = ParentGravity(time, localPosition, mass);
        //gravityVector += parentGravity;
        //Debug.DrawRay((Vector2)transform.position + localPosition, parentGravity * 100, Color.black);
        // Center Mass
        Vector2 centerGravity = OrbitMath.GravityForce(localPosition, Vector2.zero, mass, GetCenterMass());
        gravityVector += centerGravity;
        //Debug.DrawRay((Vector2)transform.position + localPosition, centerGravity * 100, Color.white);

        // Child Mass
        foreach(GravitySystem system in childSystems)
        {
            Vector2 systemVec = system.Gravity(time, localPosition);
            //Debug.DrawRay((Vector2)transform.position + localPosition, systemVec*100, Color.grey);
            //gravityVector += systemVec;
        }

        return gravityVector;
    }

    public Vector2 Gravity(float time, Vector2 localPosition,float mass=1)
    {
        Vector2 myLocalPosition  = GetPrediction(time).localPosition;
        Vector2 gravity = OrbitMath.GravityForce(localPosition, myLocalPosition, mass, GetMass());
        return gravity;
    }
    public OrbitMath.OrbitPrediction GetPrediction(float time)
    {
        //Todo Cache
        return OrbitMath.GetStaticOrbitPrediction(time, this);
    }

    /// <summary>
    /// Converstion from WorldSpace to SystemSpace
    /// </summary>
    /// <param name="time"> Time for prediction </param>
    /// <param name="position"> WorldSpace position </param>
    /// <returns></returns>
    /// 
    public OrbitMath.OrbitPrediction SetupPrediction(OrbitMath.OrbitPrediction prediction)
    {

        print("Setup prediction... " + prediction.localPosition +" " +  name + " System");
        foreach (GravitySystem childSystem in childSystems)
        {
            print(childSystem.name + ": "+ childSystem.GetPrediction(prediction.time).localPosition);
            if (childSystem.IsInSystem(prediction.time, prediction.localPosition))
            {
                prediction.localPosition = childSystem.PointFromParentSystem(prediction.time, prediction.localPosition);
                prediction.gravitySystem = childSystem;
                return childSystem.SetupPrediction(prediction);
            }
        }
        prediction.gravitySystem = this;
        return prediction;
    }
    public Vector2 PointToSystem(float time, Vector2 position)
    {
        OrbitMath.OrbitPrediction prediction = GetPrediction(time);
        if (parentSystem)
            return parentSystem.PointToSystem(time, position - prediction.localPosition);
        else
            return position;
    }

    public GravitySystem PointToGravitySystem(float time, Vector2 position)
    {
        foreach (GravitySystem childSystem in childSystems)
        {
            if (childSystem.IsInSystem(time, position))
            {
                return childSystem.PointToGravitySystem(time,childSystem.PointToSystem(time, position));
            }
        }
        return this;
    }
    public Vector2 PointToWorld(float time, Vector2 localPosition)
    {
        OrbitMath.OrbitPrediction prediction = GetPrediction(time);
        if (parentSystem)
            return parentSystem.PointToWorld(time, localPosition + prediction.localPosition);
        else
            return localPosition;
    }
    public Vector2 PointToParentSystem(float time, Vector2 localPosition)
    {
        OrbitMath.OrbitPrediction prediction = GetPrediction(time);
        if (parentSystem)
            return localPosition + prediction.localPosition;
        else
            return localPosition;
    }
    public Vector2 PointFromParentSystem(float time, Vector2 localPosition)
    {
        OrbitMath.OrbitPrediction prediction = GetPrediction(time);
        if (parentSystem)
            return localPosition - prediction.localPosition;
        else
            return localPosition;
    }

    /// <summary>
    /// In Editor
    /// </summary>
    public void CheckSystem()
    {
        mass = 0;
        if (!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();

        //Radius of Influence
        parentSystem = null;
        if (transform.parent)
        {
            parentSystem = transform.parent.GetComponent<GravitySystem>();
            
            print("TEEEESSSTTT" +name + parentSystem.name);
        }
        if (parentSystem)
        {
            float distToParentSytem = orbitElements.a_semiMajorAxis;
            radiusOfInfluence = OrbitMath.CircleOfInfluence(distToParentSytem, GetMass(), parentSystem.GetMass());

            if (!sphereOfInfluence)
            {
                sphereOfInfluence =  Instantiate(bodyPrefab, transform).GetComponent<SpriteRenderer>();
            }
            sphereOfInfluence.transform.localScale = Vector3.one * 2 * radiusOfInfluence;
            sphereOfInfluence.transform.localPosition = Vector3.zero;
            Color color = renderer.color;
            color.a = 0.1f;
            sphereOfInfluence.color = color;

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
                    print(siblingSystem.name +  " entered " + name + " system");
                    siblingSystem.transform.SetParent(transform);
                }
                if(distance < siblingSystem.radiusOfInfluence && siblingSystem.GetMass() > GetMass())
                {
                    print(name + " entered " + siblingSystem + " system");
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
                print(name + " entered " + parentSystem.transform.name + "  system");
                transform.SetParent(parentSystem.transform.parent);
            }
        }

        if (parentSystem)
        {
            localStartPosition = transform.localPosition;
            t0 = OrbitMath.GetT0(this);
        }

    }

    public void CheckOrbit()
    {
        // Only Planets
        if (!GetParentSystem())
            return;
        if (GetParentSystem().GetParentSystem())
            return;

        //Linerenderer
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer)
            return;

        //Prediction
        int count = 1000;
        float orbitTime = OrbitMath.GetOrbitPeriodA(this);
        float timeSteps = orbitTime / (count-1);
        print(timeSteps);
        Vector3[] path = new Vector3[count];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = OrbitMath.GetStaticOrbitPrediction(i * timeSteps, this,false).localPosition;
        }

        //Linerenderer Setup
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
        Color color = renderer.color;
        color.a = 0.8f;
        lineRenderer.endColor = lineRenderer.startColor = color;

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


    public GravitySystem GetFurtherSystem(Vector2 localPosition)
    {
        float distance = localPosition.magnitude;
        GravitySystem furtherSystem = null;
        foreach(GravitySystem cs in childSystems)
        {
            float csDistance = cs.localStartPosition.magnitude;
            if (!furtherSystem)
            {
                if (csDistance > distance)
                {
                    furtherSystem = cs;
                }
                continue;
            }


            Debug.DrawLine(furtherSystem.localStartPosition, cs.localStartPosition);
            if (csDistance > distance && csDistance < furtherSystem.localStartPosition.magnitude)
                furtherSystem = cs;
        }
        return furtherSystem;
    }

    public GravitySystem GetParentSystem()
    {
        if (!transform.parent)
            return null;

        return transform.parent.GetComponent<GravitySystem>();
    }
    public GravitySystem[] GetChildSystems()
    {
        List<GravitySystem> childSystems = new List<GravitySystem>();
        foreach (Transform child in transform)
        {
            GravitySystem childSystem = child.GetComponent<GravitySystem>();
            if (childSystem)
                childSystems.Add(childSystem);
        }
        return childSystems.ToArray();
    }

    /// <summary>
    /// Adds total mass together
    /// </summary>
    /// <returns> mass of all masses in system </returns>
    public override float GetMass()
    {
        if (mass > 0)
            return mass;
        mass = 0;

        mass += GetCenterMass();

        PointMass[] systemMasses = transform.GetComponentsInChildren<PointMass>();
        foreach(PointMass pm in systemMasses)
        {
            if (pm.gameObject.GetInstanceID() == GetInstanceID() || pm == this)
                continue;

            mass += pm.GetMass();
        }
        return mass;
    }

    public float GetCenterMass()
    {
        float centerMass = orbitElements.mass;
        return centerMass;
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (renderer)
            Gizmos.color = renderer.color;
        Gizmos.DrawWireSphere(transform.position, radiusOfInfluence);

        foreach(Transform child in transform)
        {
            //Gizmos.DrawLine(transform.position, child.position);
        }

        if (transform.parent)
        {
            Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        if (renderer)
            Gizmos.color = renderer.color;

        if (transform.parent)
        {
            Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) - radiusOfInfluence);
            Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) + radiusOfInfluence);
        }
    }
}