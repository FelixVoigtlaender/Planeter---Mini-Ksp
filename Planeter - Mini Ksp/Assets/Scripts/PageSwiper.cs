using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages = 1;
    private int currentPage = 1;

    public bool isInteractable = true;



    bool isDragging = false;
    Vector2 startPos;
    Vector2 endPos;
    float startTime;


    // Start is called before the first frame update
    void Start()
    {
        panelLocation = transform.position;
        startPos = transform.position;
        endPos = transform.position;
    }


    public void SetSmoothMove(Vector2 startPos, Vector2 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        startTime = Time.time;
    }

    public void Update()
    {
        if (!isDragging)
        {
            float t = (Time.time - startTime);
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
        }
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(data.position);
        Vector2 pressPosition = Camera.main.ScreenToWorldPoint(data.pressPosition);
        Debug.DrawLine(position, pressPosition);
        if (!isInteractable)
        {
            isDragging = false;
        }

        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);

        isDragging = true;
    }
    public void OnEndDrag(PointerEventData data)
    {

        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                currentPage++;
                newLocation += new Vector3(-Screen.width, 0, 0);
            }
            else if (percentage < 0 && currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            SetSmoothMove(transform.position, newLocation);
            panelLocation = newLocation;
        }
        else
        {
            SetSmoothMove(transform.position, panelLocation);
        }
        isDragging = false;
    }
}