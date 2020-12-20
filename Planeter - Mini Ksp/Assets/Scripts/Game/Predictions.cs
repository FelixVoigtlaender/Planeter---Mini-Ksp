﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Predictions
{
    public int maxI;
    public OrbitMath.OrbitPrediction[] predictions;
    //
    // Setup
    //
    public Predictions(int count)
    {
        SetupPredictions(count);
    }
    public Predictions(int count, OrbitMath.OrbitPrediction startPrediction)
    {
        SetupPredictions(count);
        SetCurrentPrediction(startPrediction);
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
    public void AddPredictionI(OrbitMath.OrbitPrediction prediction, int index)
    {
        if (!CanAddPrediction() && maxI == index)
            return;
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
        AddPredictionI(prediction, index);
    }
    public bool CanAddPrediction()
    {
        int dist = ModuloDistance(maxI, GetCurrentIndex() , predictions.Length);
        return dist > 1;
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
    public OrbitMath.OrbitPrediction GetLastPrediciton()
    {
        return GetPredictionI(GetLastIndex());
    }
    public int GetCurrentIndex()
    {
        return CheckIndexT(OTime.time);
    }
    public int GetLastIndex()
    {
        if (maxI == GetCurrentIndex())
            return GetCurrentIndex();
        return CheckIndex(maxI - 1);
    }

    public OrbitMath.OrbitPrediction GetLerpedPredicitonT(float time)
    {
        int i = CheckIndexT(time);
        int nextI = CheckIndex(i + 1);
        float percent = (time - predictions[i].time) / OTime.fixedTimeSteps;
        OrbitMath.OrbitPrediction lerpedPrediction = predictions[i].Clone();
        if(predictions[i].gravitySystem == predictions[nextI].gravitySystem)
        {
            lerpedPrediction.localVelocity = predictions[i].localVelocity + percent * (predictions[nextI].localVelocity- predictions[i].localVelocity);
            lerpedPrediction.localPosition = predictions[i].localPosition + percent * (predictions[nextI].localPosition-predictions[i].localPosition);
        }
        return lerpedPrediction;
    }

    public int CheckIndex(int index)
    {
        return (index + predictions.Length) % predictions.Length;
    }
    public int CheckIndexT(float time)
    {
        return CheckIndex(Mathf.FloorToInt(time / OTime.fixedTimeSteps));
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
