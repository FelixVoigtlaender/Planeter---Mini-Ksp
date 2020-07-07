using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class GravitySystem : PointMass
{
    public static GravitySystem sunSystem;
    [Header("My Mass")]
    public float radiusOfInfluence = 0;
    public GravitySystem parentSystem;

    public Vector2 localStartPosition;
    public float t0 = 0;

    //In Game
    GravitySystem[] childSystems;

    public void Awake()
    {
        if (transform.parent)
            parentSystem = transform.parent.GetComponent<GravitySystem>();
        else
            sunSystem = this;
    }

    private void Start()
    {
        //Orbit Prediction Setup
        localStartPosition = transform.localPosition;
        t0 = OrbitMath.GetT0(this);

        //Collect ChildSystems
        List<GravitySystem> systems = new List<GravitySystem>();
        foreach(Transform child in transform)
        {
            GravitySystem system = child.GetComponent<GravitySystem>();
            if (system)
                systems.Add(system);
        }
        childSystems = systems.ToArray();
    }


    public void FixedUpdate()
    {
        OrbitMath.OrbitPrediction prediction = OrbitMath.GetStaticOrbitPrediction(Time.time, this);
        transform.localPosition = prediction.localPosition;


    }

    public OrbitMath.OrbitPrediction DynamicPrediction(OrbitMath.OrbitPrediction prediction, float mass = 1)
    {
        Debug.DrawLine(transform.position, (Vector2)transform.position + prediction.localPosition,Color.blue);

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
    public bool IsInSystem(float time, Vector2 localPosition)
    {
        Vector2 myLocalPosition = GetPrediction(time).localPosition;
        float distance = (localPosition - myLocalPosition).magnitude;
        return distance < radiusOfInfluence;
    }

    public Vector2 TotalGravity(float time, Vector2 localPosition, float mass=1)
    {
        Vector2 gravityVector = Vector2.zero;
        int count = 0;
        
        //Center Mass
        gravityVector += OrbitMath.GravityForce(localPosition, Vector2.zero, mass, OrbitMath.MassOfCircle(radius, massScale));
        Debug.DrawRay((Vector2)transform.position + localPosition, gravityVector * 100, Color.green);
        count++;
        foreach(GravitySystem system in childSystems)
        {
            Vector2 systemVec = system.Gravity(time, localPosition);
            Debug.DrawRay((Vector2)transform.position + localPosition, systemVec*100, Color.black);

            gravityVector += systemVec;
            count++;
        }
        Debug.DrawRay((Vector2)transform.position + localPosition, gravityVector * 100, Color.white);

        Debug.DrawRay((Vector2)transform.position + localPosition, Vector2.up * gravityVector.magnitude * 100, Color.white);

        if (count == 0)
            return Vector2.zero;

        

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
    public Vector2 PointToSystem(float time, Vector2 position)
    {
        OrbitMath.OrbitPrediction prediction = GetPrediction(time);
        if (parentSystem)
            return parentSystem.PointToSystem(time, position - prediction.localPosition);
        else
            return position;
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