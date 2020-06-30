using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMass : MonoBehaviour
{
    public float massScale = 1;
    public float mass;

    public void Start()
    {
        SetUp();
    }

    public Vector2 GravityForce(Vector2 position, float mass = 1)
    {
        return OrbitMath.GravityForce(position, transform.position, this.mass, mass);
    }

    public virtual void SetUp()
    {
        if (mass > 0)
            return; //Already Setup

        float radius = transform.lossyScale.x / 2;
        mass = OrbitMath.MassOfCircle(radius, massScale);
    }
    
}
