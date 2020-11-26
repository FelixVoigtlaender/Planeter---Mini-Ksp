using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public float thrust = 1;

    public Vector2 Ignite(Vector2 direction)
    {
        gameObject.SetActive(false);
        return CalculateThrust(direction);
    }

    public Vector2 PretendIgnite(Vector2 direction)
    {
        return CalculateThrust(direction);
    }

    public Vector2 CalculateThrust(Vector2 direction)
    {
        return direction.normalized * thrust;

    }
}
