using System.Collections;
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
        startPrediction.localPosition = startPosition;
        startPrediction = GravitySystem.sunSystem.SetupPrediction(startPrediction);
        predictions.SetCurrentPrediction(startPrediction);
    }

    public void Update()
    {
        if (!GameManager.isGameActive)
            return;
     
    }


    public void FixedUpdate()
    {
        if (predictions.CanAddPrediction())
        {
            PredictPath(predictions, 10);
        }
        OrbitMath.OrbitPrediction prediction = predictions.GetLerpedPredicitonT(OTime.time);
        transform.parent = prediction.gravitySystem.transform;
        transform.localPosition = prediction.localPosition;
        DrawPath(predictionDrawer,predictions);
    }

    public void DrawPath(PredictionDrawer drawer, Predictions predictions)
    {
        if (!drawer || predictions == null)
            return;
        drawer.DrawPath(predictions);
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

    public void PredictPath(Predictions predictions, int steps)
    {
        for(int i = 0; i < steps; i++)
        {
            if (!predictions.CanAddPrediction())
                return;
            predictions.AddPredictionN(CalculateNextPrediction(predictions.GetLastPrediction()));
        }
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
        // Alters current Prediction (Reset of all predictions is handled by SetCurrentPrediction)
        OrbitMath.OrbitPrediction prediction = predictions.GetCurrentPrediction();
        prediction.localVelocity += deltaVelocity;
        predictions.SetCurrentPrediction(prediction);

        // Hides PretendPredictionDrawer
        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
    }
    public void PretendAddVelocity(Vector2 deltaVelocity)
    {
        Debug.DrawRay(transform.position, deltaVelocity * 10);

        OrbitMath.OrbitPrediction prediction = predictions.GetCurrentPrediction().Clone();
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
        OrbitMath.OrbitPrediction currentPrediction = predictions.GetCurrentPrediction();
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

        if (pretendPredictionDrawer)
            pretendPredictionDrawer.Hide();
    }


    private void OnDrawGizmos()
    {

        if (predictions == null)
            return;
        if (predictions.PredictionCount() == 0)
            return;

        OrbitMath.OrbitPrediction currentPrediction = predictions.GetCurrentPrediction();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, currentPrediction.localVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, currentPrediction.localGravity);
    }

    public OrbitMath.OrbitPrediction GetCurrentPrediction()
    {
        return predictions.GetCurrentPrediction();
    }

    public OrbitMath.OrbitPrediction GetLastPrediction()
    {
        return predictions.GetLastPrediction();
    }
}
