using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Predictions
{
    public int maxI;
    public int curI;
    public int free;
    public int filled;

    public float fixedTimeSteps;
    public OrbitMath.OrbitPrediction[] predictions;
    //
    // Setup
    //
    public Predictions(int count)
    {
        SetupPredictions(count);
        fixedTimeSteps = OTime.fixedTimeSteps;
    }
    public Predictions(int count, OrbitMath.OrbitPrediction startPrediction)
    {
        SetupPredictions(count);
        SetCurrentPrediction(startPrediction);
        fixedTimeSteps = OTime.fixedTimeSteps;
    }
    void SetupPredictions(int count)
    {
        maxI = 0;
        predictions = new OrbitMath.OrbitPrediction[count];
        for (int i = 0; i < predictions.Length; i++)
        {
            predictions[i] = new OrbitMath.OrbitPrediction();
        }
    }


    //
    // Populating
    //
    /// <summary>
    /// Puts predition at given index
    /// </summary>
    /// <param name="prediction"></param>
    /// <param name="index"></param>
    public void AddPredictionI(OrbitMath.OrbitPrediction prediction, int index, bool allowOverride = false)
    {
        if (!CanAddPrediction(index) && !allowOverride)
        {
            Debug.LogError("Couldn't add prediction");
            return;
        }
        index = CheckIndex(index);
        maxI = CheckIndex(index + 1);
        predictions[index] = prediction;
    }
    /// <summary>
    /// Puts prediction at given time
    /// </summary>
    /// <param name="prediction"></param>
    public void AddPredictionT(OrbitMath.OrbitPrediction prediction)
    {
        int index = CheckIndexT(prediction.time);
        AddPredictionI(prediction, index);
    }
    /// <summary>
    /// Puts prediction to next space
    /// </summary>
    /// <param name="prediction"></param>
    public void AddPredictionN(OrbitMath.OrbitPrediction prediction)
    {
        AddPredictionI(prediction,maxI);
    }
    public void SetCurrentPrediction(OrbitMath.OrbitPrediction prediction)
    {
        int index = CheckIndexT(OTime.time);
        AddPredictionI(prediction, index, true);
    }
    public bool CanAddPrediction(int i)
    {
        if (GetPredictionI(i) == null || GetCurrentPrediction() == null)
            return true;
        return i != GetCurrentIndex(); 
    }
    public bool CanAddPrediction()
    {
        return CanAddPrediction(maxI);
    }


    //
    // Getter
    //
    public OrbitMath.OrbitPrediction GetPredictionI(int index)
    {
        index = CheckIndex(index);
        return predictions[index];
    }
    public OrbitMath.OrbitPrediction GetPredictionT(float time)
    {
        int index = CheckIndexT(time);

        return predictions[index];
    }
    public OrbitMath.OrbitPrediction GetCurrentPrediction()
    {
        return GetPredictionT(OTime.time);
    }
    public OrbitMath.OrbitPrediction GetLastPrediction()
    {
        return GetPredictionI(GetLastIndex());
    }
    public int GetCurrentIndex()
    {
        curI = CheckIndexT(OTime.time);
        free = ModuloDistance(maxI, curI);
        return curI;
    }
    public int GetLastIndex()
    {
        if (maxI == GetCurrentIndex())
            return maxI;
        return CheckIndex(maxI - 1);
    }

    public OrbitMath.OrbitPrediction GetLerpedPredicitonT(float time)
    {
        time %= fixedTimeSteps * predictions.Length;
        int i = CheckIndexT(time);
        int nextI = CheckIndex(i + 1);
        float timeDelta = time - fixedTimeSteps * (Mathf.Floor(time / fixedTimeSteps));
        float percent = (timeDelta) / fixedTimeSteps;
        OrbitMath.OrbitPrediction lerpedPrediction = predictions[i].Clone();
        if(predictions[i].gravitySystem == predictions[nextI].gravitySystem)
        {
            lerpedPrediction.localVelocity = predictions[i].localVelocity + percent * (predictions[nextI].localVelocity - predictions[i].localVelocity);
            lerpedPrediction.localPosition = predictions[i].localPosition + percent * (predictions[nextI].localPosition - predictions[i].localPosition);
        }
        return lerpedPrediction;
    }

    public int CheckIndex(int index)
    {
        if (predictions.Length == 0)
            return 0;
        return (index + predictions.Length) % predictions.Length;
    }
    public int CheckIndexT(float time)
    {
        time %= fixedTimeSteps * predictions.Length;
        return CheckIndex(Mathf.FloorToInt(time /fixedTimeSteps));
    }
    public int Mod(int x , int m)
    {
        if (m < 0) m = -m;
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    public int ModuloDistance(int a, int b, int m)
    {
        if (a < b)
            return b - a;
        else
            return (m - a) + b;
    }
    public int ModuloDistance(int a, int b)
    {
        return ModuloDistance(a, b, predictions.Length);
    }
    public int PredictionCount()
    {
        return ModuloDistance(GetCurrentIndex(), maxI);
    }
}
