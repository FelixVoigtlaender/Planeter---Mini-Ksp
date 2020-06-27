using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBody : MonoBehaviour
{
    public Vector2 velocity = Vector2.zero;
    public float mass = 1;

    public void FixedUpdate()
    {
        velocity += GravityManager.CalculateAllGravityVector(transform.position, mass);

        Vector2 position = transform.position;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }
}
