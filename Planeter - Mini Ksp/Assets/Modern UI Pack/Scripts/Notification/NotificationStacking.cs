using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class NotificationStacking : MonoBehaviour
    {
        public List<NotificationManager> notifications = new List<NotificationManager>();
        [HideInInspector] public bool enableUpdating = false;

        [Header("SETTINGS")]
        public float delay = 1;
        public int currentNotification = 0;

        void Update()
        {
            if (enableUpdating == true)
            {
                try
                {
                    notifications[currentNotification].gameObject.SetActive(true);

                    if (notifications[currentNotification].notificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
                    {
                        notifications[currentNotification].OpenNotification();
                        StartCoroutine("StartNotification");
                        enableUpdating = false;
                    }

                    if (currentNotification >= notifications.Count)
                    {
                        enableUpdating = false;
                        currentNotification = 0;
                    }
                }

                catch
                {
                    enableUpdating = false;
                    currentNotification = 0;
                    notifications.Clear();
                }
            }
        }

        public void CloseCurrentNotification()
        {
            if (notifications.Count == 0)
                return;
            notifications[currentNotification].CloseNotification();
        }

        IEnumerator StartNotification()
        {
            if(notifications[currentNotification].enableTimer)
                yield return new WaitForSeconds(notifications[currentNotification].timer + delay);
            else
                while(!notifications[currentNotification].notificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out"))
                    yield return new WaitForSeconds(0.1f);

            Destroy(notifications[currentNotification].gameObject);
            //notifications.Remove(notifications[currentNotification]);
            currentNotification += 1;
            enableUpdating = true;
            StopCoroutine("StartNotification");
        }
    }
}