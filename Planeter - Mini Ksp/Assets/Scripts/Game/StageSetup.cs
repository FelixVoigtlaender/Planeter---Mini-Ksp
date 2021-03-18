using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

using System;

public class StageSetup : MonoBehaviour
{
    [Header("References")]
    public TMPro.TextMeshProUGUI titlePoints;
    public ReorderableList myReorderableList;
    public ReorderableList thruster;

    [Header("Points")]
    public IntValue totalPoints;
    public IntValue pointsLeft;


    public event Action<int> onPointsLeftChanged;
    public event Action<int> onTotalPointsChanged;

    private void Start()
    {
        //Added
        myReorderableList.OnElementAdded.AddListener(OnElementAdded);
        myReorderableList.OnElementDropped.AddListener(OnElementAdded);
        //Removed
        myReorderableList.OnElementRemoved.AddListener(OnElementRemoved);
        myReorderableList.OnElementGrabbed.AddListener(OnElementRemoved);


        totalPoints.Value = 3;


        pointsLeft.Value = totalPoints.Value;


        totalPoints.OnValueChanged += CheckContent;

        CheckContent();
    }
    public void OnElementAdded(ReorderableList.ReorderableListEventStruct eventStruct)
    {
        bool canAdd = CanAddStage(eventStruct);
        if (!canAdd)
        {
            Destroy(eventStruct.DroppedObject);
        }
        else
        {
            CheckContent();
        }
    }
    public void OnElementRemoved(ReorderableList.ReorderableListEventStruct eventStruct)
    {

        print("REMOVED");
        CheckContent();
    }

    public bool CanAddStage(ReorderableList.ReorderableListEventStruct eventStruct)
    {
        Stage addedStage = eventStruct.DroppedObject.GetComponent<Stage>();
        if (!addedStage)
            return true;

        return addedStage.points <= pointsLeft.Value;
    }

    public void CheckContent()
    {
        int pointsLeft = totalPoints.Value;
        // Count how much points are left
        RectTransform myContent = myReorderableList.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in myContent.transform)
        {
            if (child.gameObject.name.Contains("Fake"))
                continue;
            Stage stage = child.GetComponent<Stage>();
            if (!stage)
                continue;

            pointsLeft -= stage.points;
        }

        // Enable disable thrusters
        RectTransform thursterContent = thruster.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in thursterContent.transform)
        {
            if (child.gameObject.name.Contains("Fake"))
                continue;
            Stage stage = child.GetComponent<Stage>();
            if (!stage)
                continue;
            bool isGrabbable = pointsLeft >= stage.points;
            // Grey out
            Image image = stage.image;
            Color color = image.color;
            color.a = isGrabbable ? 1 : 0.5f;
            image.color = color;
            // Is Grabbable
            child.GetComponent<ReorderableListElement>().IsGrabbable = isGrabbable;
        }

        this.pointsLeft.Value = pointsLeft;
    }

    public void SetTotalPoints(int points)
    {
        totalPoints.Value = points;
        CheckContent();
    }

}
