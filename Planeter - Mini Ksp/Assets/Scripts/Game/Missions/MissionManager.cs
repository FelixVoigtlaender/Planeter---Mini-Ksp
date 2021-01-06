using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Display")]
    public GameObject missionDisplayPrefab;
    public Transform displayList;
    public MissionDisplay stagingDisplay;
    public UnityEvent onActiveMissionSelected;
    [Header("Missions")]
    public Mission activeMission;
    public Mission[] missions;

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
        ClearDisplayMissions();
        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].GenerateMissionEvents();
            DisplayMission(missions[i]);

        }
    }
    public void ClearDisplayMissions()
    {
        foreach(Transform child in displayList)
        {
            Destroy(child.gameObject);
        }
    }
    public void DisplayMission(Mission mission)
    {
        GameObject missionObj = Instantiate(missionDisplayPrefab, displayList);
        MissionDisplay display = missionObj.GetComponent<MissionDisplay>();
        display.Setup(mission);
    }

    public void SetActiveMission(Mission mission)
    {
        activeMission = mission;
        stagingDisplay.Setup(mission);

        onActiveMissionSelected?.Invoke();
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
    public bool achieved = false;

    public void Evaluate()
    {
        bool isMissionAchieved = true;
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
            else
            {
                isMissionAchieved = false;
            }
        }
        if (isMissionAchieved)
        {
            NotificationCreator.instance.GenerateNotification(title.ToUpper() + " ACHIEVED!");
            achieved = true;
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
        string[] missionParts = description.Split('.',',');
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
