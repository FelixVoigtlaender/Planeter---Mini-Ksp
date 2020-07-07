using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBody : MonoBehaviour
{
    public Vector2 velocity = Vector2.zero;
    public float mass = 1;

    public OrbitMath.OrbitPrediction prediction;

    public void Start()
    {
        if(prediction != null && prediction.gravitySystem)
        {
            prediction.localPosition = prediction.gravitySystem.PointToSystem(0, transform.position);
        }
    }


    public void FixedUpdate()
    {
        if (prediction == null)
            return;

        if (!prediction.gravitySystem)
            return;

        float deltaTime = Time.time - prediction.time;
        prediction.time = Time.time;

        prediction = prediction.gravitySystem.DynamicPrediction(prediction, mass);
        prediction.localVelocity += prediction.localGravity * deltaTime;
        prediction.localPosition += prediction.localVelocity * deltaTime;

        transform.parent = prediction.gravitySystem.transform;
        transform.localPosition = prediction.localPosition;
        

    }

    private void OnDrawGizmos()
    {

        if (prediction == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, prediction.localVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, prediction.localGravity);
    }
}
