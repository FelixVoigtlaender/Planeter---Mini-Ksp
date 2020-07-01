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
}
