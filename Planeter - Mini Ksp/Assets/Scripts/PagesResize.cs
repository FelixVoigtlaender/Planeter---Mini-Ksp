using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

public class PagesResize : MonoBehaviour
{
    RectTransform rect;

    public void Start()
    {
        AdjustSize();
    }
    public void AdjustSize()
    {
        if (!rect)
            rect = GetComponent<RectTransform>();

        int panelCount = Mathf.Max(1, transform.childCount);

        Vector2 size = new Vector2(Screen.width * panelCount, Screen.height);
        rect.sizeDelta = size;
    }

    private void OnGUI()
    {
        AdjustSize();
    }
}
