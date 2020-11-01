using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragManager : MonoBehaviour
{
    public bool isDragging = false;

    public Drag drag = null;

    private float dist;
    private Vector3 offset;
    private Transform toDrag;

    public static event Action<Drag> OnDragEnded;
    public static event Action<Drag> OnDragBegan;
    public static event Action<Drag> OnDrag;


    public void Start()
    {
        drag = new Drag();
    }

    void Update()
    {

        //Mouse
        bool isMouse = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0);
        Touch mouseTouch = new Touch();
        mouseTouch.position = Input.mousePosition;
        mouseTouch.phase = Input.GetMouseButton(0) ? UnityEngine.TouchPhase.Moved : mouseTouch.phase;
        mouseTouch.phase = Input.GetMouseButtonDown(0) ? UnityEngine.TouchPhase.Began : mouseTouch.phase;
        mouseTouch.phase = Input.GetMouseButtonUp(0) ? UnityEngine.TouchPhase.Ended : mouseTouch.phase;

        if (Input.touchCount != 1 && !isMouse && !drag.isDragging)
        {

            drag.isDragging = false;
            return;
        }
        


        Touch touch = isMouse ? mouseTouch : Input.touches[0];
        if (touch.phase == TouchPhase.Began)
        {
            drag.isDragging = true;
            drag.localStart = drag.localEnd = touch.position;
            OnDragBegan?.Invoke(drag);
        }
        if (drag.isDragging && touch.phase == TouchPhase.Moved)
        {
            drag.isDragging = true ;
            drag.localEnd = touch.position;

            drag.DrawDebug(Color.white);

            OnDrag?.Invoke(drag);
        }
        if (drag.isDragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            drag.localEnd = touch.position;
            drag.isDragging = false;

            drag.DrawDebug(Color.red);

            OnDragEnded?.Invoke(drag);
        }

    }

    public class Drag
    {
        public bool isDragging;
        public Vector2 localStart;
        public Vector2 localEnd;


        public Vector2 GetStart()
        {
            return Camera.main.ScreenToWorldPoint(localStart);
        }
        public Vector2 GetEnd()
        {
            return Camera.main.ScreenToWorldPoint(localEnd);
        }

        public void DrawDebug(Color color)
        {

            Debug.DrawLine(GetStart(), GetEnd(), color);
        }
    }
}
