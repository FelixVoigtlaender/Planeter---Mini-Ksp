using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelScaler : MonoBehaviour
{
    public static float Scale(float pixel, float relScreenSize = 1920f)
    {
        float pixelAspect = Camera.main.pixelHeight / relScreenSize;
        return Camera.main.orthographicSize * 2 * (pixel / (Camera.main.pixelHeight * pixelAspect));
    }
}
