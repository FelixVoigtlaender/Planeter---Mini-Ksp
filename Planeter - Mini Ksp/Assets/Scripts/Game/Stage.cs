using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stage : MonoBehaviour
{
    public float thrust = 1;
    public int points = 1;
    public Image image;
    bool quickSaveActive = true;

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
        return direction * thrust;

    }

    public void OnQuickSave()
    {
        quickSaveActive = gameObject.activeSelf;
    }
    public void OnLoadQuickSave()
    {

        gameObject.SetActive(quickSaveActive);
    }
}
