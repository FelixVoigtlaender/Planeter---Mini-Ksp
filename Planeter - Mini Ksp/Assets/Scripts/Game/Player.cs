using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public static Player instance;
    public DynamicBody dynamicBody;
    public StageManager stageManager;
    LineRenderer lineRenderer;

    public float deltaV = 1;

    Vector2 relativeVelocity;

    private void Awake()
    {
        instance = this;

        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dynamicBody = GetComponent<DynamicBody>();

        DragManager.OnDragEnded += OnLaunch;
        DragManager.OnDrag += OnPlanning;
    }

    public void Update()
    {
        if (Input.GetMouseButton(1))
        {
            print("SKIP");
            OTime.time += 1;
        }
    }


    public void OnLaunch(DragManager.Drag drag)
    {
        if (!GameManager.isGameActive)
            return;
        if (stageManager.IsStagesEmpty())
            return;
        if (drag.GetDifference().magnitude < 0.1f)
            return;

        Stage currentStage = stageManager.GetCurrentStage();

        if (!currentStage)
            return;

        Vector2 dir = drag.GetDirection();
        Vector2 thrust = currentStage.Ignite(dir);

        dynamicBody.AddVelocity(thrust);


        //Line Renderer
        lineRenderer.enabled = false;
    }

    public void OnPlanning(DragManager.Drag drag)
    {
        if (!GameManager.isGameActive)
            return;
        if (stageManager.IsStagesEmpty())
            return;

        Stage currentStage = stageManager.GetCurrentStage();

        if (!currentStage)
            return;

        Vector2 dir = drag.GetDirection();
        Vector2 thrust = currentStage.PretendIgnite(dir);
        //thrust = dir * deltaV;

        print("PLANNING: " + dir + "   " + thrust);

        dynamicBody.PretendAddVelocity(thrust);


        //Line Renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, drag.GetStart());
        lineRenderer.SetPosition(1, drag.GetEnd());
        lineRenderer.enabled = true;
    }

    public GravitySystem GetCurrentSystem()
    {
        return dynamicBody.GetCurrentPrediction().gravitySystem;
    }

}
