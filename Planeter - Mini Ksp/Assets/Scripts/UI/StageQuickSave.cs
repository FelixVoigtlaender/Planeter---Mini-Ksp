using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageQuickSave : MonoBehaviour
{
    Stage[] quicksaveStages;
    public void OnQuickSave()
    {
        quicksaveStages = GetComponentsInChildren<Stage>();
    }
    public void OnLoadQuickSave()
    {
        foreach (Stage stage in quicksaveStages)
        {
            stage.gameObject.SetActive(true);
        }
    }
}
