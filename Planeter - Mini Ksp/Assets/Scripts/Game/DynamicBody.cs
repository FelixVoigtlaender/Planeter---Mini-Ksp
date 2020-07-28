using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBody : MonoBehaviour
{
    public float mass = 1;

    public int predictionCount = 1000;

    public OrbitMath.OrbitPrediction startPrediction;



    public bool showLocal = false;
    public int maxIndex;
    public int currentIndex;
    public OrbitMath.OrbitPrediction[] predictions;

    public void Start()
    {
        if(startPrediction != null && startPrediction.gravitySystem)
        {
            startPrediction.localPosition = startPrediction.gravitySystem.PointToSystem(0, transform.position);
        }

        predictions = new OrbitMath.OrbitPrediction[predictionCount];
        for (int i = 0; i < predictions.Length; i++)
        {
            predictions[i] = new OrbitMath.OrbitPrediction();
        }
        predictions[0] = startPrediction;
    }


    public void FixedUpdate()
    {
        //Predict path
        int currentPredictionCount = ModuloDistance(currentIndex, maxIndex, predictions.Length);
        int newPredictionCount = Mathf.Max(100 - currentPredictionCount, 10);
        if (ModuloDistance(currentIndex, maxIndex, predictions.Length) < predictions.Length - newPredictionCount-2)
        {
            print("TEST");
            predictions = PredictPath(predictions, maxIndex, newPredictionCount);
            maxIndex = (maxIndex + newPredictionCount) % predictions.Length;
        }

        //
        currentIndex = (currentIndex + 1) % predictions.Length;
        OrbitMath.OrbitPrediction prediction = predictions[currentIndex];

        transform.parent = prediction.gravitySystem.transform;
        transform.localPosition = prediction.localPosition;

        DrawPath(predictions, currentIndex, maxIndex);


        Grapher.Log(prediction.localGravity.magnitude, "Gravity", prediction.time);
        Grapher.Log(prediction.localGravity.magnitude, prediction.gravitySystem.name, prediction.gravitySystem.renderer.color, prediction.time);
    }

    public void DrawPath(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        OrbitMath.OrbitPrediction enterPrediction = predictions[curI];
        Vector2 lastPosition = enterPrediction.gravitySystem.PointToWorld(enterPrediction.time, enterPrediction.localPosition);
        int switches = 0;
        for (int steps = 0; steps < ModuloDistance(curI, maxI, predictions.Length)-1; steps++)
        {
            int i = (steps + curI) % predictions.Length;
            int prevI = (i - 1 + predictions.Length) % predictions.Length;

            if (predictions[prevI].gravitySystem != predictions[i].gravitySystem)
            {
                enterPrediction = predictions[i];
                switches++;

                if (switches > 1)
                    break;
            }

            Vector2 worldPositionL = enterPrediction.gravitySystem.PointToWorld(enterPrediction.time, predictions[i].localPosition);
            Vector2 worldPositionW = enterPrediction.gravitySystem.PointToWorld(predictions[i].time, predictions[i].localPosition);
            Vector2 worldPosition = showLocal ? worldPositionL : worldPositionW;
            if (predictions[prevI].gravitySystem == predictions[i].gravitySystem)
            {
                Debug.DrawLine(lastPosition, worldPosition, enterPrediction.gravitySystem.renderer.color);
            }
            lastPosition = worldPosition;
        }
    }

    public OrbitMath.OrbitPrediction[] PredictPath(OrbitMath.OrbitPrediction[] predictions, int index, int steps)
    {
        for(int i = 0; i < steps; i++)
        {
            int nextMaxIndex = (index + 1) % predictions.Length;
            predictions[nextMaxIndex] = CalculateNextPrediction(predictions[index]);
            index = nextMaxIndex;
        }
        return predictions;
    }

    int ModuloDistance(int a, int b, int m)
    {
        if (a <= b)
            return b - a;
        else
            return (m - a) + b;
    }

    public OrbitMath.OrbitPrediction CalculateNextPrediction(OrbitMath.OrbitPrediction currentPrediction)
    {
        OrbitMath.OrbitPrediction nextPrediction = currentPrediction.Clone();


        float deltaTime = Time.fixedDeltaTime;
        nextPrediction.time += deltaTime;
        nextPrediction.localVelocity += nextPrediction.localGravity * deltaTime;
        nextPrediction.localPosition += nextPrediction.localVelocity * deltaTime;
        
        //TODO Collision

        nextPrediction = nextPrediction.gravitySystem.DynamicPrediction(nextPrediction, mass);

        return nextPrediction;
    }

    public void AddVelocity(Vector2 velocity)
    {
        predictions[currentIndex].localVelocity += velocity;
        maxIndex = currentIndex;
    }

    public void PretendAddVelocity(Vector2 velocity)
    {
        OrbitMath.OrbitPrediction[] path = new OrbitMath.OrbitPrediction[500];
        path[0] = predictions[currentIndex].Clone();
        path[0].localVelocity += velocity;

        path = PredictPath(path, 0, 499);
        DrawPath(path, 0, 499);
    }

    private void OnDrawGizmos()
    {

        if (predictions == null)
            return;
        if (predictions.Length < predictionCount)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, predictions[currentIndex].localVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, predictions[currentIndex].localGravity);
    }

    public OrbitMath.OrbitPrediction GetCurrentPrediction()
    {
        return predictions[currentIndex];
    }

    public OrbitMath.OrbitPrediction GetLastPrediction()
    {
        return predictions[maxIndex];
    }
}
