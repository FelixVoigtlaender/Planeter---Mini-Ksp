using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OMath : MonoBehaviour
{
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static OMath s_Instance = null;


    // A static property that finds or creates an instance of the manager object and returns it.
    public static OMath instance
    {
        get
        {
            if (s_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(OMath)) as OMath;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                var obj = new GameObject("Manager");
                s_Instance = obj.AddComponent<OMath>();
            }

            return s_Instance;
        }
    }


    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
    public float gravityConstant;


    /// <summary>
    /// Returns the astronomical unit(au) in km
    /// au is roughly the distance from Earth to the Sun
    /// </summary>
    /// <returns></returns>
    public static float Au()
    {
        return 149597870.700f;
    }
    /// <summary>
    /// Returns earths radius in km
    /// </summary>
    /// <returns></returns>
    public static float Er()
    {
        return 6378;
    }

    public static float Sqr(float value)
    {
        return value * value;
    }
    public static float MassOfCircle(float radius, float massScale)
    {
        float area = Mathf.PI * Mathf.Pow(radius, 2);
        float mass = area * massScale;
        return mass;
    }
    public static Vector2 GravityForce(Vector2 posA, Vector2 posB, float massA, float massB)
    {
        // From pos A to pos B
        Vector2 dif = (posB - posA);
        if (dif.sqrMagnitude < OMath.Sqr(0.1f))
            return Vector2.zero;
        float sqrDistance = dif.sqrMagnitude;
        float gravity = instance.gravityConstant * massB * massA / sqrDistance;
        Vector2 gravityVector = dif.normalized * gravity;
        return gravityVector;
    }

    public static float CircleOfInfluence(float radius, float planetMass, float sunMass)
    {
        // https://de.wikipedia.org/wiki/Einflusssph%C3%A4re_(Astronomie)
        // r * ( Mp / Ms ) ^(2/5)
        if(planetMass > sunMass)
        {
            Debug.LogError("Planet is heavier than sun???");
        }
        print("Planet: " + planetMass + " Sun: " + sunMass);


        float circleOfInfluence = radius * Mathf.Pow((planetMass / sunMass), 2f/5f);

        return circleOfInfluence;
    }

    /*
     * Circular Orbit Prediction
     * 
     * x(t)=acos( (2π(t−t0)) / T)
     * y(t)=asin( (2π(t−t0)) / T)
     * 
     * T=2π * sqrt( a^3 / q ) 
     * 
     * of the central body
     * q = G * M
    */

    public static OrbitPrediction GetStaticOrbitPredictionA(float time, GravitySystem gravitySystem)
    {
        GravitySystem parentSystem = gravitySystem.centerSystem;
        // No Central body, thus no prediction
        if (!parentSystem)
            return new OrbitPrediction(time,gravitySystem);

        float q = instance.gravityConstant * parentSystem.GetMass();
        float a = gravitySystem.localStartPosition.magnitude;
        float T = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / q);
        float t0 = gravitySystem.t0;
        float x = a * Mathf.Cos((2 * Mathf.PI * (time - t0)) / T);
        float y = a * Mathf.Sin((2 * Mathf.PI * (time - t0)) / T);

        float xV = -a * Mathf.Sin((2 * Mathf.PI * (time - t0)) / T) * (2 * Mathf.PI) / T;
        float yV = a * Mathf.Cos((2 * Mathf.PI * (time - t0)) / T) * (2 * Mathf.PI) / T;

        Vector2 localPosition = new Vector2(x, y);
        Vector2 localVelocity = new Vector2(xV,yV);

        return new OrbitPrediction(time,localPosition, localVelocity);
    }

    /// <summary>
    /// Returns for a given time the Prediction with position and velocity of a static orbit
    /// </summary>
    /// <param name="time"></param>
    /// <param name="gravitySystem"></param>
    /// <param name="setData"></param>
    /// <returns></returns>
    public static OrbitPrediction GetStaticOrbitPrediction(float time, GravitySystem gravitySystem, bool setData = false) {
        
        // Orbit elements from Gravity system
        OrbitElement orbitElements = gravitySystem.orbitElement;

        // No Prediction for Sun
        if (!gravitySystem.centerSystem)
            return new OrbitPrediction(time, gravitySystem);

        // Constants
        float mu = instance.gravityConstant * gravitySystem.centerSystem.orbitElement.mass;
        float a = orbitElements.a_semiMajorAxis;
        float e =  orbitElements.e_eccentricity;
        float M = (Mathf.Sqrt(mu / Mathf.Pow(a, 3)) * time) % (2*Mathf.PI);

        // Newton Iteration Setup
        float E_start = 5;
        float E = E_start;
        int E_iterationsMax = 250;
        /// Newton Iteration
        for(int i = 0; i < E_iterationsMax; i++)
        {
            float func =    (M - E + e * Mathf.Sin(E));
            //              ---------------------------
            float funcDer =   (-1 + e * Mathf.Cos(E));

            float newEs = E - (func / funcDer);
            if (Mathf.Abs(newEs - E) < 0.00001f)
            {
                E_iterationsMax = i;
                break;
            }
            E = newEs;
        }
        float f = 2 * Mathf.Atan(Mathf.Sqrt((1 + e) / (1 - e)) * Mathf.Tan(E / 2));
        float r = (a * (1 - Mathf.Pow(e, 2))) / (1 + (e * Mathf.Cos(f)));

        // Prediction setup
        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, Mathf.Rad2Deg * f) * Vector2.right);
        Vector2 localPosition = r * dir;
        Vector2 velocity = Vector2.Perpendicular(dir) *  Mathf.Sqrt((2 * mu / r) - (mu / a));
        OMath.OrbitPrediction prediction = new OMath.OrbitPrediction(time, localPosition, velocity);
        prediction.gravitySystem = gravitySystem.centerSystem;

        //Debug
#if UNITY_EDITOR
        if (setData)
        {
            float dataTime = time;
            Grapher.Log(Mathf.Sin(time), "SINUS" + "_" + orbitElements.name, dataTime);
            Grapher.Log(mu, "mu" + "_" + orbitElements.name, dataTime);
            Grapher.Log(a, "a" + "_" + orbitElements.name, dataTime);
            Grapher.Log(e, "e" + "_" + orbitElements.name, dataTime);
            Grapher.Log(M, "M" + "_" + orbitElements.name, dataTime);
            Grapher.Log(E, "E" + "_" + orbitElements.name, dataTime);
            Grapher.Log(f, "f" + "_" + orbitElements.name, dataTime);
            Grapher.Log(r, "r" + "_" + orbitElements.name, dataTime);
            Grapher.Log(velocity.magnitude, "vel_mag" + "_" + orbitElements.name, dataTime);

            Grapher.Log(E_iterationsMax, "E_iterations" + "_" + orbitElements.name, dataTime);
            Grapher.Log(E, "E" + "_" + orbitElements.name, dataTime);
            Grapher.Log(E_start, "E_start" + "_" + orbitElements.name, dataTime);
        }
#endif

        return prediction;

    }
    public static float GetOrbitPeriod(GravitySystem gravitySystem)
    {

        OrbitElement orbitElements = gravitySystem.orbitElement;
        if (!gravitySystem.centerSystem)
            return 0;
        float a = orbitElements.a_semiMajorAxis;
        float mu = instance.gravityConstant * gravitySystem.centerSystem.orbitElement.mass;
        float P = Mathf.Sqrt((4 * Mathf.Pow(Mathf.PI, 2) * Mathf.Pow(a, 3))/(mu));

        return P;
    }

    public static float GetT0(GravitySystem gravitySystem)
    {
        GravitySystem parentSystem = gravitySystem.centerSystem;
        // No Central body, thus no prediction
        if (!parentSystem)
            return -1;

        float x = gravitySystem.localStartPosition.x;
        float y = gravitySystem.localStartPosition.y;

        float q = instance.gravityConstant * parentSystem.GetMass();
        float a = gravitySystem.localStartPosition.magnitude;
        float T = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / q);

        float xt0 = -(Mathf.Acos(x / a) * T) / (2 * Mathf.PI);
        float yt0 = -(Mathf.Asin(y / a) * T) / (2 * Mathf.PI);

        string s = gravitySystem.name + "\n";
        s += "X: " + x + " xT0: " + xt0 + "\n";
        s += "Y: " + y + " yT0: " + yt0 + "\n";

        float t0;
        t0 = y > 0 ? xt0 : yt0;
        t0 = y < 0 && x < 0 ? -xt0: t0;

        return t0;
    }

    public static int ModuloDistance(int a, int b, int m)
    {
        if (a <= b)
            return b - a;
        else
            return (m - a) + b;
    }

    public static int BetweenLineAndCircle(Vector2 circleCenter, float circleRadius, Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
    {
        float t;

        var dx = point2.x - point1.x;
        var dy = point2.y - point1.y;

        var a = dx * dx + dy * dy;
        var b = 2 * (dx * (point1.x - circleCenter.x) + dy * (point1.y - circleCenter.y));
        var c = (point1.x - circleCenter.x) * (point1.x - circleCenter.x) + (point1.y - circleCenter.y) * (point1.y - circleCenter.y) - circleRadius * circleRadius;

        var determinate = b * b - 4 * a * c;
        if ((a <= 0.0000001) || (determinate < -0.0000001))
        {
            // No real solutions.
            intersection1 = Vector2.zero;
            intersection2 = Vector2.zero;
            return 0;
        }
        if (determinate < 0.0000001 && determinate > -0.0000001)
        {
            // One solution.
            t = -b / (2 * a);
            intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = Vector2.zero;
            return 1;
        }

        // Two solutions.
        t = (float)((-b + Mathf.Sqrt(determinate)) / (2 * a));
        intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
        t = (float)((-b - Mathf.Sqrt(determinate)) / (2 * a));
        intersection2 = new Vector2(point1.x + t * dx, point1.y + t * dy);

        return 2;
    }



    [System.Serializable]
    public class OrbitPrediction
    {
        public float time;
        public Vector2 localPosition;
        public Vector2 localVelocity;
        public Vector2 localGravity;
        public GravitySystem gravitySystem;
        public bool isGrounded = false;

        public OrbitPrediction(float time,Vector2 localPosition, Vector2 localVelocity)
        {
            this.time = time;
            this.localPosition = localPosition;
            this.localVelocity = localVelocity;
        }
        public OrbitPrediction(float time,GravitySystem gs)
        {
            this.time = time;
            this.localPosition = gs.transform.localPosition;
            this.localVelocity = Vector2.zero;
        }
        public OrbitPrediction()
        {

        }


        public void SetPrediction(OrbitPrediction prediction)
        {
            time = prediction.time;
            localPosition = prediction.localPosition;
            localVelocity = prediction.localVelocity;
            localGravity = prediction.localGravity;
            gravitySystem = prediction.gravitySystem;
            isGrounded = prediction.isGrounded;

        }
        public OrbitPrediction Clone()
        {
            OrbitPrediction clone = new OrbitPrediction(time, localPosition, localVelocity);
            clone.SetPrediction(this);
            return clone;
        }

        public void ChangeSystem(GravitySystem newSystem)
        {
            // Same System
            if (gravitySystem == newSystem)
                return;

            if (!newSystem)
                return;

            OrbitPrediction newSystemPrediction = newSystem.GetPrediction(time);

            // From World Space
            if (!gravitySystem)
            {
                if (!newSystem.centerSystem)
                {
                    //Sun
                    gravitySystem = newSystem;
                    return;
                }
                else
                {
                    //
                    localPosition -= newSystemPrediction.localPosition;
                    localVelocity -= newSystemPrediction.localVelocity;
                    gravitySystem = newSystem;
                    return;
                }
            }

            // From ParentSystem
            if (gravitySystem == newSystem.centerSystem)
            {
                localPosition -= newSystemPrediction.localPosition;
                localVelocity -= newSystemPrediction.localVelocity;
                gravitySystem = newSystem;
                return;
            }
            // From Child System
            if (gravitySystem.centerSystem == newSystem)
            {

                OMath.OrbitPrediction childPrediction = gravitySystem.GetPrediction(time);
                localPosition += childPrediction.localPosition;
                localVelocity += childPrediction.localVelocity;
                gravitySystem = newSystem;

                return;
            }

            gravitySystem = newSystem;
        }

    }
}
