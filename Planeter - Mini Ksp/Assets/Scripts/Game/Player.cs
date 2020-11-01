using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public static Player instance;
    public DynamicBody dynamicBody;

    public float deltaV = 1;

    Vector2 relativeVelocity;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        dynamicBody = GetComponent<DynamicBody>();

        DragManager.OnDragEnded += OnLaunch;
        DragManager.OnDrag += OnPlanning;
    }


    public void OnLaunch(DragManager.Drag drag)
    {
        Vector2 dragDif = drag.localStart - drag.localEnd;
        Vector2 dragDir = dragDif.normalized;

        dynamicBody.AddVelocity(dragDir * deltaV);


        Time.timeScale = 1;
    }

    public void OnPlanning(DragManager.Drag drag)
    {
        Vector2 dragDif = drag.localStart - drag.localEnd;
        Vector2 dragDir = dragDif.normalized;

        dynamicBody.PretendAddVelocity(dragDir * deltaV);

        Time.timeScale = 0.5f;
    }

    public GravitySystem GetCurrentSystem()
    {
        return dynamicBody.GetCurrentPrediction().gravitySystem;
    }

}
