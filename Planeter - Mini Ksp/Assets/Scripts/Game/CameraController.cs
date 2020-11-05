using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;



    [Header("Movement")]
    public bool lockX;
    public float horizontalSmoothTime;
    public float verticalSmoothTime;
    public Vector2 offset;
    float smoothVelocityX;
    float smoothVelocityY;
    Vector3 middle;

    [Header("Size")]
    public float minimumSize = 3;
    public float maximumSize = 100;
    public float skinWidth = 1;
    float newOSize;
    float smoothVelocitySize;
    public Vector2 size;

    public bool lockSize;
    public float sizeSmoothTime;
    float smoothVelocity0Size;

    Player player;
    GravitySystem currentSystem;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (!currentSystem)
            return;
        //Move to position
        Vector3 focusPosition = middle;
        focusPosition.z = transform.position.z;
        focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, horizontalSmoothTime);
        focusPosition.x = lockX ? 0 : focusPosition.x;
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        transform.position = (Vector3)focusPosition;

        //Size 
        float oSize =  Mathf.SmoothDamp(Camera.main.orthographicSize, newOSize, ref smoothVelocitySize, sizeSmoothTime,1000,Time.fixedUnscaledDeltaTime);
        Camera.main.orthographicSize = Mathf.Clamp(oSize, minimumSize,maximumSize);
    }

    public void FixedUpdate()
    {
        currentSystem = player.GetCurrentSystem();

        //Position
        middle = currentSystem.transform.position;

        //Size
        size = Vector2.one * currentSystem.radiusOfInfluence * 2 * 1.2f;
        PredictionDrawer predictionDrawer = player.dynamicBody.predictionDrawer;
        if(predictionDrawer.systemCount <= 1 || true)
        {
            middle = currentSystem.transform.position;
            size = Vector2.one * currentSystem.radiusOfInfluence * 2 * 1.2f;
            if (!currentSystem.parentSystem)
            {
                //print("SUN");
                GravitySystem furtherSystem = currentSystem.GetFurtherSystem(player.transform.localPosition);
                if (furtherSystem)
                {

                    //print("SUNNY: " + furtherSystem.name);
                    size = Vector2.one * furtherSystem.localStartPosition.magnitude * 2 * 1.2f;
                }
            }
            else
            {
                //print("One System");
            }
        }
        else if(predictionDrawer.systemCount > 1){
            //print("TRANSFER");
            //middle = (predictionDrawer.min + predictionDrawer.max) / 2;
            //size = (predictionDrawer.max - predictionDrawer.min) * 1.2f;
        }

        newOSize = ToOrthographicSize(size);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, size);
    }

    public float ToOrthographicSize(Vector2 size)
    {
        float oSize = 0;
        size = new Vector2(size.x + skinWidth, size.y + skinWidth);
        size = size * 1;

        if (size.x > size.y * Camera.main.aspect)
        {
            size.y = size.x / Camera.main.aspect;
        }

        oSize = size.y / 2;

        return oSize;
    }
}
