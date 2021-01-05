using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static MissionManager s_Instance = null;
    // A static property that finds or creates an instance of the manager object and returns it.
    public static MissionManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(MissionManager)) as MissionManager;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                var obj = new GameObject("Manager");
                s_Instance = obj.AddComponent<MissionManager>();
            }

            return s_Instance;
        }
    }

    public Mission[] missions;
    public Mission activeMission;

    private void Start()
    {
        GenerateMissions();
        GameManager.OnGameStart += Reset;
        GameManager.OnGameStart += Evaluate;
    }

    public void GenerateNotification(MissionEvent missionEvent)
    {
        string title = missionEvent.Title();
        string description = missionEvent.Description();
        NotificationCreator.instance.GenerateNotification(title, description);
    }
    public void GenerateMissions()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].GenerateMissionEvents();
        }
        // Testing
        SetActiveMission(missions[0]);
    }
    public void SetActiveMission(Mission mission)
    {
        activeMission = mission;
    }

    public void Evaluate()
    {
        if (activeMission == null || !GameManager.isGameActive)
            return;
        activeMission.Evaluate();
    }
    public void Reset()
    {
        if (activeMission == null)
            return;

        activeMission.Reset();
    }

}

[System.Serializable]
public class Mission
{
    public string title;
    [TextArea]
    public string description;
    public MissionEvent[] missionEvents;

    public void Evaluate()
    {
        for (int i = 0; i < missionEvents.Length; i++)
        {
            MissionEvent missionEvent = missionEvents[i];
            // Already Achieved
            if (missionEvent.achieved)
                continue;

            // Evaluate
            missionEvent.SelfEvaluate();
            // Notify player
            if (missionEvent.achieved)
            {
                MissionManager.instance.GenerateNotification(missionEvent);
            }
        }
    }
    public void Reset()
    {
        for (int i = 0; i < missionEvents.Length; i++)
        {
            MissionEvent missionEvent = missionEvents[i];
            missionEvent.Reset();
        }
    }
    /// <summary>
    /// Generates the MissionEvents from the description
    /// </summary>
    public void GenerateMissionEvents()
    {
        string[] missionParts = description.Split('\n');
        List<MissionEvent> events = new List<MissionEvent>();
        string[] planets = OrbitBodyGenerator.instance.GetPlanetNames();
        MissionEvent[] allEvents = MissionEvent.GetAllMissionEvents();

        for (int i = 0; i < missionParts.Length; i++)
        {
            for (int j = 0; j < allEvents.Length; j++)
            {
                MissionEvent missionEvent = allEvents[j].GenerateFromString(missionParts[i]);
                if (missionEvent == null)
                    continue;
                events.Add(missionEvent); 
            }
        }

        missionEvents = events.ToArray();
    }
}
