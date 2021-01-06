using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionDisplay : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    Mission mission;

    public void Setup(Mission mission)
    {
        this.mission = mission;
        title.text = mission.title;
        description.text = mission.description;
    }

    public void OnSelect()
    {
        MissionManager.instance.SetActiveMission(mission);
    }
}
