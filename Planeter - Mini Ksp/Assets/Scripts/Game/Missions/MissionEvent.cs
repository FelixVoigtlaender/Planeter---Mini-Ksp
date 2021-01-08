using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MissionEvent
{
    // Mission Parameters
    public virtual string MyVerb { get; set; }
    public string myObject;
    public bool achieved = false;

    // Evaluation
    public virtual bool Evaluate()
    {
        return false;
    }
    public virtual void Reset()
    {
        achieved = false;
    }
    public virtual bool SelfEvaluate()
    {
        achieved = achieved ? achieved : Evaluate();
        return achieved;
    }

    // Display
    public virtual string Title()
    {
        return MyVerb.ToUpper() + " " +  myObject.ToUpper();
    }
    public virtual string Description()
    {
        return MyVerb + " " + myObject;
    }
    public virtual void GenerateMyObject()
    {

        OrbitMath.OrbitPrediction prediction = Player.instance.dynamicBody.GetCurrentPrediction();
        myObject = prediction.gravitySystem.name;
    }

    public virtual MissionEvent GenerateFromString(string description) 
    { 
        return null; 
    }

    public static MissionEvent[] GetAllMissionEvents()
    {
        MissionEvent[] events = { new MissionLand(), new MissionEnter() };
        return events;
    }
}

[System.Serializable]
public class MissionLand : MissionEvent
{
    // Parameters
    public override string MyVerb { get; set; } = "land";

    // Evaluation
    public override bool Evaluate()
    {
        OrbitMath.OrbitPrediction prediction = Player.instance.dynamicBody.currentPrediction;

        bool correctSystem = prediction.gravitySystem.orbitElement.name == myObject;
        bool landed = prediction.isGrounded;

        return correctSystem && landed;
    }
    public override MissionEvent GenerateFromString(string description)
    {
        if (!description.Contains(MyVerb))
            return null;

        string[] planetNames = OrbitBodyGenerator.instance.GetPlanetNames();
        string planet = "";
        for (int i = 0; i < planetNames.Length; i++)
        {
            if (description.Contains(planetNames[i]))
            {
                planet = planetNames[i];
                break;
            }
        }
        if (planet.Length == 0)
            return null;

        MissionLand missionEvent = new MissionLand();
        missionEvent.myObject = planet;
        return missionEvent;
    }


    // Display
    public override string Title()
    {
        return ("Touchdown").ToUpper();
    }
    public override string Description()
    {
        return ("You landed on " + myObject);
    }


}


[System.Serializable]
public class MissionEnter : MissionEvent
{
    // Parameters
    public override string MyVerb { get; set; } = "enter";

    // Evaluation
    public override bool Evaluate()
    {
        OrbitMath.OrbitPrediction prediction = Player.instance.dynamicBody.currentPrediction;

        bool correctSystem = prediction.gravitySystem.orbitElement.name == myObject;

        return correctSystem;
    }
    public override MissionEvent GenerateFromString(string description)
    {
        if (!description.Contains(MyVerb))
            return null;

        string[] planetNames = OrbitBodyGenerator.instance.GetPlanetNames();
        string planet = "";
        for (int i = 0; i < planetNames.Length; i++)
        {
            if (description.Contains(planetNames[i]))
            {
                planet = planetNames[i];
                break;
            }
        }
        if (planet.Length == 0)
            return null;

        MissionEnter missionEvent = new MissionEnter();
        missionEvent.myObject = planet;
        return missionEvent;
    }
    // Display
    public override string Title()
    {
        return (myObject).ToUpper();
    }
    public override string Description()
    {
        return ("You entered the " + myObject + " system");
    }
}