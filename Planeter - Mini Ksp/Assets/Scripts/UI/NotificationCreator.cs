using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack; // MUIP namespace required

public class NotificationCreator : MonoBehaviour
{
    public GameObject notificationPrefab;
    public float delay = 1;
    public void GenerateNotification(string title)
    {
        GenerateNotification(title, "", null);
    }
    public void GenerateNotification(string title, string description)
    {
        GenerateNotification(title, description, null);
    }
    public void GenerateNotification(string title, string description, Sprite icon)
    {
        GameObject notificationObj = Instantiate(notificationPrefab, transform);
        NotificationManager notification = notificationObj.GetComponent<NotificationManager>();

        notification.UpdateUI();
        notification.title = title;
        notification.description = description;
        notification.icon = icon ? icon : notification.icon;

        notification.OpenNotification();

        StartCoroutine("DestroyNotification",notification);
    }

    IEnumerator DestroyNotification(NotificationManager notification)
    {
        yield return new WaitForSeconds(notification.timer + delay);
        Destroy(notification.gameObject); 
        yield break;
    }
}
