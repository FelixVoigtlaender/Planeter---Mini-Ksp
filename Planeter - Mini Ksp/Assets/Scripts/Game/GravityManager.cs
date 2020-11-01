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
            if (((Vector2)body.transform.position - position).magnitude < 1)
                continue;


            count++;
            gravityVector += OrbitMath.GravityForce(position, body.transform.position, mass, body.mass);
        }
        if (count == 0)
            return Vector2.zero;

        gravityVector /= count;
        return gravityVector;
    }

}
