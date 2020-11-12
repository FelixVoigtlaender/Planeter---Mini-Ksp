using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizePixelScaler : MonoBehaviour
{
    public float pixelSize = 10;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = Vector3.one * PixelScaler.Scale(pixelSize);
    }
}
