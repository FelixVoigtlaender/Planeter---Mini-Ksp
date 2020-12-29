using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShadow : MonoBehaviour
{

    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;
    Transform planetTransform;
    public RectTransform rectTransform;
    public float rotationOffset = 0;

    private void Awake()
    {
        planetTransform = transform.parent.parent;
        rectTransform = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 position = rectTransform ? -rectTransform.localPosition : -transform.position;
        position = position.magnitude > 0 ? position : Vector3.down;
        transform.right = -position;

        Quaternion rotation = transform.rotation;
        rotation *= Quaternion.Euler(0,0,rotationOffset);
        transform.rotation = rotation;

        // float angle = Vector2.Angle(Vector2.up, planetTransform.position);
        // transform.rotation = Quaternion.Euler(0, 0, angle);

        // // _direction = planetTransform.position.x >= 0 ? (Vector3.zero - planetTransform.position).normalized : (planetTransform.position - Vector3.zero).normalized;
        // _direction = (Vector3.zero - planetTransform.position).normalized ;

        // float rot_z = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        //find the vector pointing from our position to the target
        // //create the rotation we need to be in to look at the target
        // _lookRotation = Quaternion.LookRotation(_direction);
        // _lookRotation.z = _lookRotation.x;
        // _lookRotation.x = 0;
        // _lookRotation.y = 0;
        // //rotate us over time according to speed until we are in the required rotation
        // transform.rotation = _lookRotation;
    }
}
