using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitMath : MonoBehaviour
{
    public static OrbitMath instance;
    public float gravityConstant;

    private void Awake()
    {
        instance = this;
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
        float distance = dif.magnitude;
        float gravity = instance.gravityConstant * massB * massA / Mathf.Pow(distance, 2);
        Vector2 gravityVector = dif.normalized * gravity;
        return gravityVector;
    }

    public static float CircleOfInfluence(float radius, float planetMass, float sunMass)
    {
        // https://de.wikipedia.org/wiki/Einflusssph%C3%A4re_(Astronomie)
        // r * ( Mp / Ms ) ^(2/5)
        if(planetMass > sunMass)
        {
            print("Planet is heavier than sun???");
        }
        print("Planet: " + planetMass + " Sun: " + sunMass);


        float circleOfInfluence = radius * Mathf.Pow((planetMass / sunMass), 0.6f);

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

    public static OrbitPrediction GetStaticOrbitPrediction(float t, GravitySystem gravitySystem)
    {
        GravitySystem parentSystem = gravitySystem.parentSystem;
        // No Central body, thus no prediction
        if (!parentSystem)
            return new OrbitPrediction(gravitySystem);

        float q = instance.gravityConstant * parentSystem.GetMass();
        float a = gravitySystem.localStartPosition.magnitude;
        float T = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / q);
        float t0 = gravitySystem.t0;
        float x = a * Mathf.Cos((2 * Mathf.PI * (t - t0)) / T);
        float y = a * Mathf.Sin((2 * Mathf.PI * (t - t0)) / T);

        Vector2 localPosition = new Vector2(x, y);
        Vector2 localVelocity = Vector2.zero;

        return new OrbitPrediction(localPosition, localVelocity);
    }

    public static float GetT0(GravitySystem gravitySystem)
    {
        GravitySystem parentSystem = gravitySystem.parentSystem;
        // No Central body, thus no prediction
        if (!parentSystem)
            return 0;

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
        print(s);

        float t0 = 0;
        t0 = y > 0 ? xt0 : yt0;
        t0 = y < 0 && x < 0 ? -xt0: t0;

        return t0;
    }

    public class OrbitPrediction
    {
        public Vector2 localPosition;
        public Vector2 localVelocity;

        public OrbitPrediction(Vector2 localPosition, Vector2 localVelocity)
        {
            this.localPosition = localPosition;
            this.localVelocity = localVelocity;
        }
        public OrbitPrediction(GravitySystem gs)
        {
            this.localPosition = gs.transform.localPosition;
            this.localVelocity = Vector2.zero;
        }
    }



}
