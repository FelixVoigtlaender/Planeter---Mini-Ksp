using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionDisplay : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI points;
    public Animator iconAnimator;

    Mission mission;

    public void Setup(Mission mission)
    {
        this.mission = mission;
        title.text = mission.title;
        description.text = mission.description;
        points.text = mission.pointReward + "";
        // Icon
        mission.onAchieved += CheckAchieved;
        CheckAchieved();
    }

    public void OnSelect()
    {
        MissionManager.instance.SetActiveMission(mission);
    }

    public void CheckAchieved()
    {
        if(iconAnimator && mission.achieved)
            iconAnimator.Play("In");
    }
}
