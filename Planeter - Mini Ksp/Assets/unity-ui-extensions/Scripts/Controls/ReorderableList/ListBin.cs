using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ListBin : MonoBehaviour
{
    ReorderableList reorderableList;

    private void Awake()
    {
        reorderableList = GetComponent<ReorderableList>();
        reorderableList.OnElementDropped.AddListener(BinItem);
    }

    public void BinItem(ReorderableList.ReorderableListEventStruct listEvent)
    {
        print("BIN!");
    }
}
