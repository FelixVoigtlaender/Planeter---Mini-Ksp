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
    public Joystick joystick;

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
    }

    public void FixedUpdate()
    {
        OnPlanning();
    }


    public void OnLaunch()
    {
        if (!GameManager.isGameActive)
            return;
        if (stageManager.IsStagesEmpty())
            return;
        Vector2 dir = joystick.Direction;
        print(dir);

        Vector2 thrust = stageManager.Ignite(dir);

        dynamicBody.AddRelativeVelocity(thrust);

        // Mission - Cut
        //MissionManager.instance.Evaluate();
    }

    public void OnPlanning()
    {
        if (!GameManager.isGameActive)
            return;
        if (stageManager.IsStagesEmpty())
        {
            if (dynamicBody.pretendPredictionDrawer)
            {
                dynamicBody.pretendPredictionDrawer.Hide();
            }
            return;
        }

        Stage currentStage = stageManager.GetCurrentStage();

        if (!currentStage)
            return;

        Vector2 dir = joystick.Direction;
        Vector2 thrust = currentStage.PretendIgnite(dir);
        //thrust = dir * deltaV;


        dynamicBody.PretendAddRelativeVelocity(thrust);
    }

    public GravitySystem GetCurrentSystem()
    {
        return dynamicBody.GetCurrentPrediction().gravitySystem;
    }

}
