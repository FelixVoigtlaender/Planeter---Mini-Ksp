using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DynamicBody : MonoBehaviour
{
    public float mass = 1;

    public int predictionCount = 2000;
    public int pretendPredictionCount = 2000;

    public OMath.OrbitPrediction startPrediction;
    public OMath.OrbitPrediction currentPrediction;
    public OMath.OrbitPrediction prevPrediction;
    public OMath.OrbitPrediction quicksavePrediction;



    public bool showLocal = false;
    public Predictions predictions;
    public Predictions pretendPredictions;
    public PredictionDrawer predictionDrawer;
    public PredictionDrawer pretendPredictionDrawer;


    Transform startParent;
    Vector2 startPosition;
    

    public void Start()
    {
        startParent = transform.parent;
        startPosition = transform.position;


        //Init predictions
        predictions = new Predictions(predictionCount);
        pretendPredictions = new Predictions(pretendPredictionCount);

        Reset();

        GameManager.OnQuicksave += OnQuickSave;
        GameManager.OnLoadQuickSave += OnLoadQuickSave;
        GameManager.OnGameStart += Reset;

        IEnumerator enumerator = PathPrediction();
        StartCoroutine(enumerator);
    }



    public void Reset()
    {
        transform.parent = startParent;
        transform.position = startPosition;

        Setup();
    }

    public void Setup()
    {
        // Init StartPrediction
        startPrediction.localPosition = startPrediction.gravitySystem.orbitElement.radius * 1.1f * Vector2.up;
        startPrediction.localVelocity = Vector2.zero;
        predictions.SetCurrentPrediction(startPrediction);
    }

    public void Update()
    {
        if (!GameManager.isGameActive)
            return;
     
    }


    public void FixedUpdate()
    {
        prevPrediction.SetPrediction(currentPrediction);
        currentPrediction = predictions.GetLerpedPredicitonT(OTime.time);
        if (transform.parent != currentPrediction.gravitySystem.transform)
            transform.parent = currentPrediction.gravitySystem.transform;

        transform.localPosition = currentPrediction.localPosition;
        DrawPath(predictionDrawer,predictions);

        // Mission - Cut
        /*if (prevPrediction.isGrounded != currentPrediction.isGrounded)
            MissionManager.instance.Evaluate();
        if (prevPrediction.gravitySystem != currentPrediction.gravitySystem)
            MissionManager.instance.Evaluate();*/
    }

    private IEnumerator PathPrediction()
    {
        while (true)
        {
            if (predictions.CanAddPrediction())
            {
                PredictPath(predictions, Mathf.CeilToInt(10 * OTime.timeScale));
            }
            OTime.maxTime = predictions.GetLastPrediction().time;
            yield return new WaitForEndOfFrame();
        }
    }

    public void DrawPath(PredictionDrawer drawer, Predictions predictions)
    {
        if (!drawer || predictions == null)
            return;
        drawer.DrawPath(predictions);
    }

    public Vector2 RelativeTimePositionToWorld(OMath.OrbitPrediction curPrediciton, List<OMath.OrbitPrediction> entryPredictions)
    {
        if(curPrediciton.gravitySystem.centerSystem == null)
        {
            return curPrediciton.localPosition;
        }

        foreach(OMath.OrbitPrediction entryPred in entryPredictions)
        {
            if (curPrediciton.gravitySystem == entryPred.gravitySystem)
            {
                Vector2 relativePosition = entryPred.gravitySystem.PointToParentSystem(entryPred.time, curPrediciton.localPosition);
                OMath.OrbitPrediction relativePred = new OMath.OrbitPrediction(curPrediciton.time, relativePosition, curPrediciton.localVelocity);
                relativePred.gravitySystem = entryPred.gravitySystem.centerSystem;
                return RelativeTimePositionToWorld(relativePred, entryPredictions);
            }
        }
        return curPrediciton.gravitySystem.PointToWorld(entryPredictions[0].time,curPrediciton.localPosition);
    }

    public void PredictPath(Predictions predictions, int steps)
    {
        OMath.OrbitPrediction lastPrediction;
        OMath.OrbitPrediction nextPrediction;
        for (int i = 0; i < steps; i++)
        {
            if (!predictions.CanAddPrediction())
                return;

            lastPrediction = predictions.GetLastPrediction();
            nextPrediction = predictions.GetMaxPrediction();
            CalculateNextPrediction(lastPrediction, nextPrediction);
            predictions.AddPredictionI(nextPrediction, predictions.GetMaxIndex());
        }
    }

    public OMath.OrbitPrediction CalculateNextPrediction(OMath.OrbitPrediction currentPrediction, OMath.OrbitPrediction nextPrediction)
    {
        // Setup
        nextPrediction.SetPrediction(currentPrediction);
        // Movement
        float deltaTime = OTime.fixedTimeSteps;
        nextPrediction.time += deltaTime;
        nextPrediction.localVelocity += nextPrediction.localGravity * deltaTime;
        nextPrediction.localPosition += nextPrediction.localVelocity * deltaTime;

        // Collision
        if (nextPrediction.localPosition.sqrMagnitude < OMath.Sqr(nextPrediction.gravitySystem.radius+0.1f))
        {
            bool fliesIntoCenter = Vector2.Dot(nextPrediction.localPosition.normalized, nextPrediction.localVelocity) < 0;
            if (nextPrediction.localPosition.sqrMagnitude < OMath.Sqr(nextPrediction.gravitySystem.radius + 0.01f))
            {
                Vector2 point1 = nextPrediction.localPosition;
                Vector2 point2 = currentPrediction.localPosition;
                Vector2 intersection1 = Vector2.zero;
                Vector2 intersection2;
                int intersectionCount = OMath.BetweenLineAndCircle(Vector2.zero, nextPrediction.gravitySystem.radius, point1, point2, out intersection1, out intersection2);

                if (intersectionCount > 0)
                {
                    Vector2 position = intersection1;
                    nextPrediction.localPosition = position;
                    nextPrediction.localVelocity = Vector2.zero;
                }
            }
            nextPrediction.isGrounded = true;
        }
        else
        {
            nextPrediction.isGrounded = false;
        }

        nextPrediction.gravitySystem.DynamicPrediction(nextPrediction, mass);

        return nextPrediction;
    }
    /// <summary>
    /// Adds veloticity in worldspace
    /// </summary>
    /// <param name="deltaVelocity"></param>
    public void AddVelocity(Vector2 deltaVelocity)
    {
        // Alters current Prediction (Reset of all predictions is handled by SetCurrentPrediction)
        OMath.OrbitPrediction prediction = predictions.GetCurrentPrediction();
        prediction.localVelocity += deltaVelocity;
        predictions.SetCurrentPrediction(prediction);

        // Hides PretendPredictionDrawer
        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
    }
    public void PretendAddVelocity(Vector2 deltaVelocity)
    {
        Debug.DrawRay(transform.position, deltaVelocity * 10);

        OMath.OrbitPrediction prediction = pretendPredictions.GetCurrentPrediction();
        prediction.SetPrediction(predictions.GetCurrentPrediction());
        prediction.localVelocity += deltaVelocity;
        pretendPredictions.SetCurrentPrediction(prediction);

        PredictPath(pretendPredictions, pretendPredictions.predictions.Length);
        DrawPath(pretendPredictionDrawer, pretendPredictions);
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
        OMath.OrbitPrediction currentPrediction = predictions.GetCurrentPrediction();
        Vector2 myDir = currentPrediction.isGrounded ? currentPrediction.localPosition : currentPrediction.localVelocity;
        myDir.Normalize();
        return myDir * relativeDelta.y  - Vector2.Perpendicular(myDir) * relativeDelta.x;
    }

    /// <summary>
    /// Save Load System
    /// </summary>
    /// 
    public void OnQuickSave()
    {
        quicksavePrediction = predictions.GetCurrentPrediction().Clone();
    }
    public void OnLoadQuickSave()
    {
        predictions.SetCurrentPrediction(quicksavePrediction);
        PredictPath(predictions, 10);


        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
    }


    private void OnDrawGizmos()
    {

        if (predictions == null)
            return;
        if (predictions.PredictionCount() == 0)
            return;

        OMath.OrbitPrediction currentPrediction = predictions.GetCurrentPrediction();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, currentPrediction.localVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, currentPrediction.localGravity);
    }

    public OMath.OrbitPrediction GetCurrentPrediction()
    {
        return predictions.GetCurrentPrediction();
    }

    public OMath.OrbitPrediction GetLastPrediction()
    {
        return predictions.GetLastPrediction();
    }
}
