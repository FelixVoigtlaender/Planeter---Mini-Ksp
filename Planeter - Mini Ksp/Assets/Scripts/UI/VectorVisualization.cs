using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VectorVisualization : MonoBehaviour
{
    public Vector2 vector;
    public RectTransform handle;
    private RectTransform baseRect;

    private void Start()
    {
        baseRect = GetComponent<RectTransform>();
    }
}
