using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions.Tweens;

public class OTime : MonoBehaviour
{
    public static float time;
    public static float fixedTimeSteps = 0.1f;
    public static float deltaTime = 0.01f;
    public static float timeScale = 1;
    public static bool isPaused = false;

    private void FixedUpdate()
    {
        if (isPaused)
            return;

        if (!GameManager.isGameActive)
            return;
        //deltaTime = Time.fixedDeltaTime;

        time += deltaTime * timeScale;
    }

    public void Skip(float delta)
    {
        time += delta;
    }

    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    public void TogglePause()
    {
        SetPause(!isPaused);
    }
    public void SetPause(bool value)
    {
        isPaused = value;
    }
}
