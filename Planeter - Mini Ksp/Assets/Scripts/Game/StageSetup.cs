using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI.Extensions;

public class StageSetup : MonoBehaviour
{
    [Header("References")]
    public TMPro.TextMeshProUGUI titlePoints;
    public ReorderableList myReorderableList;
    [Header("Points")]
    public int totalPoints = 10;
    int pointsLeft;


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
        pointsLeft = totalPoints;
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
        UpdatePointText();
    }

    public void UpdatePointText()
    {
        titlePoints.text = pointsLeft + " POINTS";
        print("Points changed");
    }
}
