using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody : MonoBehaviour
{
    public float massScale = 1;
    public float mass;

    public void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        float radius = transform.lossyScale.x / 2;
        mass = CalculateMass(radius, massScale);
    }
    public static float CalculateMass(float radius, float massScale)
    {
        float area = Mathf.PI * Mathf.Pow(radius, 2);
        float mass = area * massScale;
        return mass;
    }
}
