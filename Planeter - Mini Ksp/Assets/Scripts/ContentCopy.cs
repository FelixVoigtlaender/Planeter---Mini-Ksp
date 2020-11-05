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
        myReorderableList.OnElementAdded.AddListener(ContentChanged);
        myReorderableList.OnElementRemoved.AddListener(ContentChanged);
        myReorderableList.OnElementGrabbed.AddListener(ContentChanged);
        myReorderableList.OnElementDropped.AddListener(ContentChanged);
    }

    public void ContentChanged(ReorderableList.ReorderableListEventStruct eventStruct)
    {
        print("CHANGED");
        ClearContent();
        FillContent();
    }
    public void ClearContent()
    {
        RectTransform content = reorderableList.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in content.transform)
        {
            Destroy(child);
        }
    }
    public void FillContent()
    {
        RectTransform myContent = myReorderableList.ContentLayout.GetComponent<RectTransform>();
        RectTransform content = reorderableList.ContentLayout.GetComponent<RectTransform>();
        foreach (Transform child in myContent.transform)
        {
            Instantiate(child, content);
        }
    }
}
