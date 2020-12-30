﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMass : MonoBehaviour
{
    [Header("Point Mass - information")]
    public float radius = 1;
    public float massScale = 1;
    public float mass;
    public SpriteRenderer renderer;

    [Header("Point Mass - Setup")]
    public Transform body;
    public GameObject bodyPrefab;


    public Vector2 GravityForce(Vector2 position, float mass = 1)
    {
        return OrbitMath.GravityForce(position, transform.position, this.mass, mass);
    }

    public Transform GetBody()
    {
        if (!body)
        {
            GameObject bodyObject = Instantiate(bodyPrefab, transform);
            body = bodyObject.transform;
        }
        renderer = renderer ? renderer : body.GetComponent<SpriteRenderer>();
        body.localPosition = Vector3.zero;
        body.localScale = Vector3.one * radius * 2;
        return body;
    }
    
    public virtual float GetMass()
    {
        //mass = OrbitMath.MassOfCircle(radius, massScale);
        return mass;
    }
}
