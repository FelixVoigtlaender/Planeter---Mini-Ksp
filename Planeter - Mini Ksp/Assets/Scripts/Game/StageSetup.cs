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
    public int totalPoints = 10;
    public int pointsLeft;


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

        pointsLeft = totalPoints;
        CheckContent();
        UpdatePointText();
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

        return addedStage.points <= pointsLeft;
    }

    public void CheckContent()
    {
        int pointsLeft = totalPoints;
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
            bool isGrabbable = pointsLeft > stage.points;
            // Grey out
            Image image = stage.image;
            Color color = image.color;
            color.a = isGrabbable ? 1 : 0.5f;
            image.color = color;
            // Is Grabbable
            child.GetComponent<ReorderableListElement>().IsGrabbable = isGrabbable;
        }
        if (this.pointsLeft != pointsLeft)
            onPointsLeftChanged?.Invoke(pointsLeft);


        this.pointsLeft = pointsLeft;
        UpdatePointText();
    }

    public void SetTotalPoints(int points)
    {
        if (totalPoints != points)
            onTotalPointsChanged?.Invoke(points);

        totalPoints = points;
        CheckContent();
    }

    public void UpdatePointText()
    {
        titlePoints.text = pointsLeft + " POINTS";
        print("Points changed");
    }
}
