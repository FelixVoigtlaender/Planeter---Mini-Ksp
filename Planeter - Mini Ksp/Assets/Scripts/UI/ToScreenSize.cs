using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToScreenSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetSize();
    }

    public void SetSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        rectTransform.sizeDelta = size;
    }
}
