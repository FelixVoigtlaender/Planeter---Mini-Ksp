using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Linq;

public class GravitySystem : PointMass
{
    public static GravitySystem sunSystem;
    [Header("My Mass")]
    public float radiusOfInfluence = 0;
    public GravitySystem centerSystem;
    public SpriteRenderer sphereOfInfluence;

    public Vector2 localStartPosition;
    public float t0 = 0;

    public OrbitElement orbitElement = new OrbitElement();

    public LineRenderer lineRenderer; 


    public Predictions predictions;
    //In Game
    public GravitySystem[] childSystems;

    public void Awake()
    {

        base.mass = orbitElement.mass;
        if (transform.parent)
            centerSystem = transform.parent.GetComponent<GravitySystem>();
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
        t0 = OMath.GetT0(this);

        //Draw Orbit
        CheckOrbit();
    }

    private void Start()
    {
        CheckSystem();
    }


    public void FixedUpdate()
    {
        OMath.OrbitPrediction prediction = GetPrediction(OTime.time);
        transform.localPosition = prediction.localPosition;


        if (lineRenderer)
        {
            // Color
            GradientColorKey colorStart = new GradientColorKey(renderer.color, 0);
            GradientColorKey colorEnd = new GradientColorKey(renderer.color, 1);
            // Alpha
            float totalTime = (predictions.predictions.Length) * predictions.fixedTimeSteps;
            float time = OTime.time % totalTime;
            float percent = time / totalTime;
            percent = predictions.GetCurrentIndex() / (float)predictions.predictions.Length;
            float maxAlpha = 1f;
            float minAlpha = 0.0f;
            float difAlpha = maxAlpha - minAlpha;
            GradientAlphaKey alphaMid0 = new GradientAlphaKey(maxAlpha,( percent) % 1.0f);
            GradientAlphaKey alphaMid1 = new GradientAlphaKey(minAlpha, (percent + 0.000001f) % 1.0f);
            GradientAlphaKey alphaStart = new GradientAlphaKey((1-percent)*difAlpha+minAlpha, 0);
            GradientAlphaKey alphaEnd = new GradientAlphaKey((1-percent)*difAlpha+minAlpha, 1);
            // Gradient
            Gradient gradient = new Gradient();
            gradient.colorKeys = new GradientColorKey[] { colorStart, colorEnd };
            gradient.alphaKeys = new GradientAlphaKey[] { alphaMid0, alphaMid1, alphaStart, alphaEnd };
            lineRenderer.colorGradient = gradient;
        }
    }

    public OMath.OrbitPrediction DynamicPrediction(OMath.OrbitPrediction prediction, float mass = 1)
    {
        //Set Prediction to this type
        prediction.ChangeSystem(this);

        //Outside of system
        if (prediction.localPosition.sqrMagnitude > OMath.Sqr(radiusOfInfluence) && centerSystem)
            return centerSystem.DynamicPrediction(prediction, mass);

        //Inside of child system
        for (int i = 0; i < childSystems.Length; i++)
        {
            GravitySystem childSystem = childSystems[i];
            if (childSystem.IsInSystem(prediction.time, prediction.localPosition))
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
        float sqrDistanceToCenter = parentLocalPosition.sqrMagnitude;
        // Out of furthest path
        if (sqrDistanceToCenter > OMath.Sqr(orbitElement.a_semiMajorAxis + radiusOfInfluence))
            return false;
        if (sqrDistanceToCenter < OMath.Sqr(orbitElement.a_semiMajorAxis * (1 - orbitElement.e_eccentricity) - radiusOfInfluence))
            return false;

        Vector2 myLocalPosition = GetPrediction(time).localPosition;
        float sqrDistance = (parentLocalPosition - myLocalPosition).sqrMagnitude;
        return sqrDistance < OMath.Sqr(radiusOfInfluence);
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
        if (!centerSystem)
            return Vector2.zero;

        Vector2 myLocalPosition = GetPrediction(time).localPosition;
        Vector2 parentLocalPosition = localPosition + myLocalPosition;
        float parentMass = centerSystem.GetCenterMass();
        Vector2 gravity = OMath.GravityForce(parentLocalPosition,Vector2.zero,mass,parentMass);


        return gravity + centerSystem.ParentGravity(time, parentLocalPosition, mass);
    }
    

    public Vector2 TotalGravity(float time, Vector2 localPosition, float mass=1)
    {
        Vector2 gravityVector = Vector2.zero;
        // Parent Mass
        //Vector2 parentGravity = ParentGravity(time, localPosition, mass);
        //gravityVector += parentGravity;
        //Debug.DrawRay((Vector2)transform.position + localPosition, parentGravity * 100, Color.black);
        // Center Mass
        Vector2 centerGravity = OMath.GravityForce(localPosition, Vector2.zero, mass, GetCenterMass());
        gravityVector += centerGravity;
        //Debug.DrawRay((Vector2)transform.position + localPosition, centerGravity * 100, Color.white);

        // Child Mass
        /*for (int i = 0; i < childSystems.Length; i++)
        {
            Vector2 systemVec = childSystems[i].Gravity(time, localPosition);
        }*/

        return gravityVector;
    }

    public Vector2 Gravity(float time, Vector2 localPosition,float mass=1)
    {
        Vector2 myLocalPosition  = GetPrediction(time).localPosition;
        Vector2 gravity = OMath.GravityForce(localPosition, myLocalPosition, mass, GetMass());
        return gravity;
    }
    public OMath.OrbitPrediction GetPrediction(float time)
    {
        if (!centerSystem)
            return new OMath.OrbitPrediction(time, transform.localPosition, Vector2.zero);

        return predictions.GetLerpedPredicitonT(time);
    }

    /// <summary>
    /// Converstion from WorldSpace to SystemSpace
    /// </summary>
    /// <param name="time"> Time for prediction </param>
    /// <param name="position"> WorldSpace position </param>
    /// <returns></returns>
    /// 
    public OMath.OrbitPrediction SetupPrediction(OMath.OrbitPrediction prediction)
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
        OMath.OrbitPrediction prediction = GetPrediction(time);
        if (centerSystem)
            return centerSystem.PointToSystem(time, position - prediction.localPosition);
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
        OMath.OrbitPrediction prediction = GetPrediction(time);
        if (centerSystem)
            return centerSystem.PointToWorld(time, localPosition + prediction.localPosition);
        else
            return localPosition;
    }
    public Vector2 PointToParentSystem(float time, Vector2 localPosition)
    {
        OMath.OrbitPrediction prediction = GetPrediction(time);
        if (centerSystem)
            return localPosition + prediction.localPosition;
        else
            return localPosition;
    }
    public Vector2 PointFromParentSystem(float time, Vector2 localPosition)
    {
        OMath.OrbitPrediction prediction = GetPrediction(time);
        if (centerSystem)
            return localPosition - prediction.localPosition;
        else
            return localPosition;
    }


    public void Setup(OrbitElement element)
    {
        this.orbitElement = element;

        //Name
        gameObject.name = element.name;

        // Center System
        centerSystem =  GameObject.Find(element.center).GetComponent<GravitySystem>();
        transform.SetParent(centerSystem.transform);

        // Position - Puts planet at its Periapsis
        transform.localPosition = Vector3.right * element.a_semiMajorAxis *(1- element.e_eccentricity);

        // Body
        body = GetBody();
        body.localScale = Vector3.one * 2 * element.radius;
        Color color = new Color();
        ColorUtility.TryParseHtmlString(element.color, out color);
        renderer.color = color;

        mass = element.mass;
        radius = element.radius;
    }

    /// <summary>
    /// In Editor
    /// </summary>
    public void CheckSystem()
    {
        CheckChildSystems();


        mass = 0;
        if (!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();

        //Radius of Influence
        centerSystem = null;
        if (transform.parent)
        {
            centerSystem = transform.parent.GetComponent<GravitySystem>();
            
            print("TEEEESSSTTT" +name + centerSystem.name);
        }
        if (centerSystem)
        {
            float distToParentSytem = orbitElement.a_semiMajorAxis;
            radiusOfInfluence = OMath.CircleOfInfluence(distToParentSytem, GetMass(), centerSystem.GetMass());

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
            //radiusOfInfluence = 10000000000;
        }

        // Add Siblings in RadiusOfInfluence
        if (centerSystem)
        {
            GravitySystem[] siblingSystems = centerSystem.GetChildSystems();
            foreach(GravitySystem siblingSystem in siblingSystems)
            {
                if (siblingSystem == this)
                    continue;

                float sqrDistance = ((Vector2)transform.position- (Vector2)siblingSystem.transform.position).sqrMagnitude;
                if(sqrDistance < OMath.Sqr(radiusOfInfluence) && siblingSystem.GetMass() < GetMass())
                {
                    print(siblingSystem.name +  " entered " + name + " system");
                    siblingSystem.transform.SetParent(transform);
                }
                if(sqrDistance < OMath.Sqr(siblingSystem.radiusOfInfluence) && siblingSystem.GetMass() > GetMass())
                {
                    print(name + " entered " + siblingSystem + " system");
                    transform.SetParent(siblingSystem.transform);
                }
            }
        }

        // Check if this system exited another System
        if (centerSystem)
        {
            float distance = Vector2.Distance(transform.position, centerSystem.transform.position);
            if(distance > centerSystem.radiusOfInfluence)
            {
                print(name + " entered " + centerSystem.transform.name + "  system");
                transform.SetParent(centerSystem.transform.parent);
            }
        }

        if (centerSystem)
        {
            localStartPosition = transform.localPosition;
            t0 = OMath.GetT0(this);
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
        lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer)
            return;

        //Prediction
        float orbitTime = OMath.GetOrbitPeriod(this);
        //int stepCount = Mathf.FloorToInt(orbitTime / OTime.fixedPlanetTimeSteps);
        int stepCount = 360;
        predictions = new Predictions(stepCount);
        predictions.fixedTimeSteps = (orbitTime / (float) (stepCount));
        Vector3[] path = new Vector3[stepCount];
        for (int i = 0; i < stepCount; i++)
        {
            OMath.OrbitPrediction prediction = OMath.GetStaticOrbitPrediction(i * predictions.fixedTimeSteps, this, false);
            predictions.AddPredictionI(prediction,i,true);
            path[i] = prediction.localPosition;
        }

        //Linerenderer Setup
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
        Color color = renderer.color;
        color.a = 0.1f;
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
        float sqrDistance = localPosition.sqrMagnitude;
        GravitySystem furtherSystem = null;
        foreach(GravitySystem cs in childSystems)
        {
            float sqrCsDistance = cs.localStartPosition.sqrMagnitude;
            if (!furtherSystem)
            {
                if (sqrCsDistance > sqrDistance)
                {
                    furtherSystem = cs;
                }
                continue;
            }


            Debug.DrawLine(furtherSystem.localStartPosition, cs.localStartPosition);
            if (sqrCsDistance > sqrDistance && sqrCsDistance < furtherSystem.localStartPosition.sqrMagnitude)
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
        float centerMass = orbitElement.mass;
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
            //Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) - radiusOfInfluence);
            //Gizmos.DrawWireSphere(transform.parent.position, Vector2.Distance(transform.position, transform.parent.position) + radiusOfInfluence);
        }
    }
}