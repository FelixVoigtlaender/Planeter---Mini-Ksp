using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ContentCopy : MonoBehaviour
{
    public ReorderableList myReorderableList;
    public ReorderableList reorderableList;

    public void Start()
    {
        myReorderableList = GetComponent<ReorderableList>();
    }

    public void OnElementAdded(ReorderableList.ReorderableListEventStruct eventStruct)
    {
        ClearContent();
        FillContent();
    }
    public void OnElementRemoved(ReorderableList.ReorderableListEventStruct eventStruct)
    {
        ClearContent();
        FillContent(eventStruct.SourceObject);
    }

    public void ClearContent()
    {
        RectTransform content = reorderableList.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void FillContent(GameObject except = null)
    {
        RectTransform myContent = myReorderableList.ContentLayout.GetComponent<RectTransform>();
        RectTransform content = reorderableList.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in myContent.transform)
        {
            if (child.gameObject == except)
                continue;
            if (child.gameObject.name.Contains("Fake"))
                continue;
            Instantiate(child, content);
        }
    }

}
