using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack; // MUIP namespace required

public class NotificationCreator : MonoBehaviour
{
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static NotificationCreator s_Instance = null;
    // A static property that finds or creates an instance of the manager object and returns it.
    public static NotificationCreator instance
    {
        get
        {
            if (s_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(NotificationCreator)) as NotificationCreator;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                var obj = new GameObject("Manager");
                s_Instance = obj.AddComponent<NotificationCreator>();
            }

            return s_Instance;
        }
    }



    public GameObject notificationPrefab;
    public GameObject missionEventNoficiationPrefab;
    public NotificationStacking notificationStacking;
    public float delay = 1;

    public void GenerateNotification(string title)
    {
        GenerateNotification(title, "", null);
    }
    public void GenerateNotificationDescrition(string title)
    {
        GenerateNotification(title, title, null);
    }
    public void GenerateNotification(string title, string description)
    {
        GenerateNotification(title, description, null);
    }

    public NotificationManager GenerateNotification(string title, string description, Sprite icon)
    {
        GameObject notificationObj = Instantiate(notificationPrefab, transform);
        NotificationManager notification = notificationObj.GetComponent<NotificationManager>();

        notification.title = title;
        notification.description = description;
        notification.icon = icon ? icon : notification.icon;
        notification.UpdateUI();

        return notification;
    }

    public void ClearNotifications()
    {
        notificationStacking.ClearNotifications();

    }

    public void GenerateMissionEventNotification(MissionEvent missionEvent)
    {
        GameObject notificationObj = Instantiate(missionEventNoficiationPrefab, transform);
        MissionNotification notification = notificationObj.GetComponent<MissionNotification>();
        notification.SetUp(missionEvent);
    }

    IEnumerator DestroyNotification(NotificationManager notification)
    {
        yield return new WaitForSeconds(notification.timer + delay);
        Destroy(notification.gameObject); 
        yield break;
    }
}
