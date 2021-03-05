using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack; // MUIP namespace required

public class MissionNotification : MonoBehaviour
{
    [Header("Display")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Animator iconAnimator;



    MissionEvent missionEvent;
    NotificationManager notification;
    
    public void SetUp(MissionEvent missionEvent)
    {
        this.missionEvent = missionEvent;
        notification = GetComponent<NotificationManager>();

        // Display Information
        title.text = missionEvent.Title();
        description.text = missionEvent.Description();


        missionEvent.onAchieved += CheckAchieved;
    }

    public void CheckAchieved()
    {
        if (iconAnimator && iconAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out") && missionEvent.achieved)
        {
            iconAnimator.Play("In");
            notification.Invoke("CloseNotification", 1f);

        }
    }
}
