using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OTime : MonoBehaviour
{
    public static float time;
    public static float fixedDeltaTime = 0.1f;
    public static float timeScale = 1;

    private void FixedUpdate()
    {
        time += fixedDeltaTime * timeScale;
    }
}
