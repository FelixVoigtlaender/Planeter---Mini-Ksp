using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody : MonoBehaviour
{
    public float massScale = 1;
    public float mass;

    public void Start()
    {
        mass = CalculateMass();
    }

    public float CalculateMass()
    {
        //A = π r2

        float radius = transform.lossyScale.x / 2;
        float area = Mathf.PI * Mathf.Pow(radius, 2);

        float mass = area * massScale;
        return mass;
    }
}
