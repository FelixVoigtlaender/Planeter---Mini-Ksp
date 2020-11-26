﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DynamicBody : MonoBehaviour
{
    public float mass = 1;

    public int predictionCount = 2000;
    public int pretendPredictionCount = 2000;

    public OrbitMath.OrbitPrediction startPrediction;
    public OrbitMath.OrbitPrediction currentPrediction;
    public OrbitMath.OrbitPrediction quicksavePrediction;



    public bool showLocal = false;
    public int maxIndex;
    public int currentIndex;
    public OrbitMath.OrbitPrediction[] predictions;
    public PredictionDrawer predictionDrawer;
    public PredictionDrawer pretendPredictionDrawer;


    Transform startParent;
    Vector2 startPosition;
    

    public void Start()
    {
        startParent = transform.parent;
        startPosition = transform.position;

        Reset();

        GameManager.OnQuicksave += OnQuickSave;
        GameManager.OnLoadQuickSave += OnLoadQuickSave;
        GameManager.OnGameEnd += Reset;
    }

    public void Reset()
    {
        transform.parent = startParent;
        transform.position = startPosition;

        Setup();
    }

    public void Setup()
    {
        maxIndex = currentIndex = 0;


        // Init StartPrediction
        startPrediction.localPosition = startPosition;
        startPrediction = GravitySystem.sunSystem.SetupPrediction(startPrediction);

        predictions = new OrbitMath.OrbitPrediction[predictionCount];
        for (int i = 0; i < predictions.Length; i++)
        {
            predictions[i] = new OrbitMath.OrbitPrediction();
        }
        predictions[0] = startPrediction;
    }


    public void FixedUpdate()
    {
        if (!GameManager.isGameActive)
            return;
        //Predict path
        int currentPredictionCount = ModuloDistance(currentIndex, maxIndex, predictions.Length);
        int newPredictionCount = Mathf.Max(100 - currentPredictionCount, 10);
        if (ModuloDistance(currentIndex, maxIndex, predictions.Length) < predictions.Length - newPredictionCount-2)
        {
            predictions = PredictPath(predictions, maxIndex, newPredictionCount);
            maxIndex = (maxIndex + newPredictionCount) % predictions.Length;
        }

        //
        //currentIndex = (currentIndex + 1) % predictions.Length;
        //

        int skipSteps = (int)((OTime.time - predictions[currentIndex].time) / OTime.fixedTimeSteps);
        currentIndex = (currentIndex + skipSteps) % predictions.Length;
        OrbitMath.OrbitPrediction prediction = predictions[currentIndex];
        OrbitMath.OrbitPrediction nextPrediction = predictions[(currentIndex + 1) % predictions.Length];

        float percent = (OTime.time - prediction.time) / OTime.fixedTimeSteps;
        percent = Mathf.Clamp01(percent);
        Vector2 localTweenPosition = Vector2.Lerp(prediction.localPosition, nextPrediction.localPosition, percent);

        transform.parent = prediction.gravitySystem.transform;
        transform.localPosition = localTweenPosition;

        DrawPath(predictions, currentIndex, maxIndex);


#if UNITY_EDITOR
        this.currentPrediction = prediction;
        Grapher.Log(prediction.localGravity.magnitude, "Gravity", prediction.time);
        Grapher.Log(prediction.localGravity.magnitude, prediction.gravitySystem.name, prediction.gravitySystem.renderer.color, prediction.time);
#endif
    }

    public void DrawPath(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        if (predictionDrawer)
        {
            predictionDrawer.DrawPrediction(predictions, curI, maxI);
            return;
        }
    }

    public void PretendDrawPath(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        if (pretendPredictionDrawer)
        {
            pretendPredictionDrawer.DrawPrediction(predictions, curI, maxI);
            return;
        }
    }

    public Vector2 RelativeTimePositionToWorld(OrbitMath.OrbitPrediction curPrediciton, List<OrbitMath.OrbitPrediction> entryPredictions)
    {
        if(curPrediciton.gravitySystem.parentSystem == null)
        {
            return curPrediciton.localPosition;
        }

        foreach(OrbitMath.OrbitPrediction entryPred in entryPredictions)
        {
            if (curPrediciton.gravitySystem == entryPred.gravitySystem)
            {
                Vector2 relativePosition = entryPred.gravitySystem.PointToParentSystem(entryPred.time, curPrediciton.localPosition);
                OrbitMath.OrbitPrediction relativePred = new OrbitMath.OrbitPrediction(curPrediciton.time, relativePosition, curPrediciton.localVelocity);
                relativePred.gravitySystem = entryPred.gravitySystem.parentSystem;
                return RelativeTimePositionToWorld(relativePred, entryPredictions);
            }
        }
        return curPrediciton.gravitySystem.PointToWorld(entryPredictions[0].time,curPrediciton.localPosition);
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
        

        // Movement
        float deltaTime = OTime.fixedTimeSteps;
        nextPrediction.time += deltaTime;
        nextPrediction.localVelocity += nextPrediction.localGravity * deltaTime;
        nextPrediction.localPosition += nextPrediction.localVelocity * deltaTime;

        // Collision
        if (nextPrediction.localPosition.magnitude < nextPrediction.gravitySystem.radius)
        {
            bool fliesIntoCenter = Vector2.Dot(nextPrediction.localPosition.normalized, nextPrediction.localVelocity) < 0;
            if (fliesIntoCenter)
            {
                Vector2 position = nextPrediction.localPosition.normalized * nextPrediction.gravitySystem.radius;
                nextPrediction.localPosition = position;
                nextPrediction.localVelocity = Vector2.zero;
                nextPrediction.isGrounded = true;
            }
        }

        nextPrediction = nextPrediction.gravitySystem.DynamicPrediction(nextPrediction, mass);

        return nextPrediction;
    }
    /// <summary>
    /// Adds veloticity in worldspace
    /// </summary>
    /// <param name="deltaVelocity"></param>
    public void AddVelocity(Vector2 deltaVelocity)
    {
        predictions[currentIndex].localVelocity += deltaVelocity;
        maxIndex = currentIndex;

        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
    }
    public void PretendAddVelocity(Vector2 velocity)
    {
        Debug.DrawRay(transform.position, velocity * 10);

        OrbitMath.OrbitPrediction[] path = new OrbitMath.OrbitPrediction[pretendPredictionCount];
        path[0] = predictions[currentIndex].Clone();
        path[0].localVelocity += velocity;

        path = PredictPath(path, 0, path.Length-1);
        PretendDrawPath(path, 0, path.Length-1);
    }
    /// <summary>
    /// Adds relative velocity to the current velocity thus that
    /// y: Parallel to current velocity
    /// x: Orthogonal to current velocity
    /// </summary>
    /// <param name="relativeDelta"></param>
    public void AddRelativeVelocity(Vector2 relativeDelta)
    {
        AddVelocity(RelativeToWorldVelocity(relativeDelta));
    }
    public void PretendAddRelativeVelocity(Vector2 relativeDelta)
    {
        PretendAddVelocity(RelativeToWorldVelocity(relativeDelta));
    }
    public Vector2 RelativeToWorldVelocity(Vector2 relativeDelta)
    {
        Vector2 myDir = predictions[currentIndex].isGrounded ? predictions[currentIndex].localPosition : predictions[currentIndex].localVelocity;
        myDir.Normalize();
        return myDir * relativeDelta.y  - Vector2.Perpendicular(myDir) * relativeDelta.x;
    }

    /// <summary>
    /// Save Load System
    /// </summary>
    /// 
    public void OnQuickSave()
    {
        quicksavePrediction = predictions[currentIndex].Clone();
    }
    public void OnLoadQuickSave()
    {
        predictions[currentIndex] = quicksavePrediction;
        maxIndex = currentIndex;

        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
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
