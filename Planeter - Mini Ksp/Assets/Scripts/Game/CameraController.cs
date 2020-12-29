using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;



    [Header("Movement")]
    public Transform target;
    public Vector2 targetDelta;
    public Vector2 targetOffset;
    public float maxOffset = 500f;
    public float horizontalSmoothTime;
    public float verticalSmoothTime;
    Vector2 targetSmoothVel;

    [Header("Size")]
    public float sizeMin = 3;
    public float sizeMax = 100;
    public float sizeGoal = 4;
    float smoothVelocitySize;
    public float sizeSmoothTime = 0.1f;
    public float zoomScale = 0.1f;

    [Header("Input")]
    public FieldTrigger fieldTrigger;

    private void Awake()
    {
        instance = this;

        if (fieldTrigger)
        {
            fieldTrigger.onDrag += OnDrag;
            //fieldTrigger.onEndDrag += OnEndDrag;
            //fieldTrigger.onPointerDown += OnPointerDown;
            //fieldTrigger.onPointerUp += OnPointerUp;
        }
    }

    private void Update()
    {
        FollowTarget();
        FollowSize();
        MouseInput();
    }


    public void MouseInput()
    {
        float deltaZoom = 0;
        //Mouse Zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            deltaZoom = -100;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            deltaZoom = 100;
        }
        // Mouse move Camera
        if (Input.GetMouseButtonDown(0))
        {
        }

        DeltaZoom(deltaZoom);
    }
    public void ManageDrag(PointerEventData eventData)
    {

        zoomScale = sizeGoal / (50 * 10);

        float deltaZoom = 0;

        if (Input.touchCount >= 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Zoom
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            Vector2 firstPrevPos = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondPrevPos = secondTouch.position - secondTouch.deltaPosition;

            float touchesPrevPosDif = (firstPrevPos - secondPrevPos).magnitude;
            float touchesCurPosDif = (firstTouch.position - secondTouch.position).magnitude;


            float zoomDelta = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomScale;

            if (touchesPrevPosDif > touchesCurPosDif)
                deltaZoom = zoomDelta;
            if (touchesPrevPosDif < touchesCurPosDif)
                deltaZoom = -zoomDelta;
        }
        else if(eventData.clickCount == 1)
        {
            // Move Camera
            Vector2 startPosition = Camera.main.ScreenToWorldPoint(eventData.position + eventData.delta);
            Vector2 endPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            Vector2 worldDelta = endPosition - startPosition;
            targetOffset += worldDelta;
            if(targetOffset.magnitude > maxOffset)
            {
                targetOffset = targetOffset.normalized * maxOffset;
            }
        }

        
        DeltaZoom(deltaZoom);
    }
    public void DeltaZoom(float delta)
    {
        zoomScale = sizeGoal / (50 * 10);
        sizeGoal += delta * zoomScale;
        sizeGoal = Mathf.Clamp(sizeGoal, sizeMin, sizeMax);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ManageDrag(eventData);
        print(eventData.position + " " + eventData.delta);
    }
    void FollowTarget()
    {
        // Nothing to follow
        if (!target)
            return;

        //Decrease Delta
        targetDelta.x = Mathf.SmoothDamp(targetDelta.x, 0, ref targetSmoothVel.x, horizontalSmoothTime);
        targetDelta.y = Mathf.SmoothDamp(targetDelta.y, 0, ref targetSmoothVel.y, verticalSmoothTime);

        // Init Position
        Vector3 position = (Vector2)target.position + targetOffset + targetDelta;
        position.z = transform.position.z;

        transform.position = position;
    }
    void FollowSize()
    {
        float oSize = Mathf.SmoothDamp(Camera.main.orthographicSize, sizeGoal, ref smoothVelocitySize, sizeSmoothTime);
        Camera.main.orthographicSize = Mathf.Clamp(oSize, sizeMin, sizeMax);
    }


    public static void SetTarget(Transform target)
    {
        instance.target = target;

        if (!target)
            return;

        instance.targetOffset = Vector2.zero;
        instance.targetDelta = (Vector2)instance.transform.position - ((Vector2)target.position) ;
    }
    public static void LockTarget(Transform target)
    {
        instance.target = target;

        if (!target)
            return;

        instance.targetOffset = Vector2.zero;
        instance.targetDelta = Vector2.zero;
    }
}
