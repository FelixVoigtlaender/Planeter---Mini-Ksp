using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static GravityManager instance;
    public List<SolarBody> bodies = new List<SolarBody>();
    public float gravityConstant = 1f;

    public void Awake()
    {
        instance = this;
        bodies.AddRange(FindObjectsOfType<SolarBody>());

    }


    public static Vector2 CalculateAllGravityVector(Vector2 position, float mass, SolarBody exlcude = null)
    {


        Vector2 gravityVector = Vector2.zero;
        int count = 0;
        foreach(SolarBody body in instance.bodies)
        {
            if (body == exlcude)
                continue;
            if (((Vector2)body.transform.position - position).magnitude < 0.1)
                continue;


            count++;
            gravityVector += CalculateGravityVector(position, body.transform.position, mass, body.mass);
        }
        gravityVector /= count;
        return gravityVector;
    }
    public static Vector2 CalculateGravityVector(Vector2 posA, Vector2 posB, float massA, float massB)
    {
        // From pos A to pos B
        Vector2 dif = (posB - posA);
        float distance = dif.magnitude;

        float gravity = instance.gravityConstant * massB * massA / Mathf.Pow(distance, 2);

        Vector2 gravityVector = dif.normalized * gravity;
        return gravityVector;
    }
}
