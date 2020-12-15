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

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        FollowTarget();
        FollowSize();
    }

    public void ManagePointerClick()
    {

    }
    public void ManageDrag()
    {

        zoomScale = sizeGoal / (50 * 10);


        if (Input.touchCount >= 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            Vector2 firstPrevPos = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondPrevPos = secondTouch.position - secondTouch.deltaPosition;

            float touchesPrevPosDif = (firstPrevPos - secondPrevPos).magnitude;
            float touchesCurPosDif = (firstTouch.position - secondTouch.position).magnitude;


            float zoomDelta = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomScale;

            if (touchesPrevPosDif > touchesCurPosDif)
                sizeGoal += zoomDelta;
            if (touchesPrevPosDif < touchesCurPosDif)
                sizeGoal -= zoomDelta;
        }

        //Mouse
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            sizeGoal -= zoomScale * 100;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            sizeGoal += zoomScale * 100;
        }

        sizeGoal = Mathf.Clamp(sizeGoal, sizeMin, sizeMax);

    }

    public void OnDrag(PointerEventData eventData)
    {
        print("DRAGGING");
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        print("DRAG END");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print("POINTER DOWN");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("POINTER UP");
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

        instance.targetDelta = (Vector2)instance.transform.position - ((Vector2)target.position) ;
    }
    public static void LockTarget(Transform target)
    {
        instance.target = target;

        if (!target)
            return;

        instance.targetDelta = Vector2.zero;
    }
}
