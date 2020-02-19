using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    // event for starting a "run" state
    public static UnityEvent onStartRun = new UnityEvent();

    // event for ending a "run" state
    public static UnityEvent onEndRun = new UnityEvent();

    // event for beginning the "run stats" state
    public static UnityEvent onStartRunStats = new UnityEvent();

    // event for ending the game
    [System.Serializable]
    public class EndGameEvent : UnityEvent<EndGame.Reason> { };
    public static EndGameEvent onEndGame = new EndGameEvent();

    // called when turn gets changed
    public static UnityEvent onTurnUpdated = new UnityEvent();

    // called when a new generation of fish has been created
    [System.Serializable]
    public class NewGenerationEvent : UnityEvent<List<FishGenome>, List<FishGenome>> { };
    public static NewGenerationEvent onNewGeneration = new NewGenerationEvent();

    // called when a change in fish population has occurred
    [System.Serializable]
    public class PopulationEvent : UnityEvent<List<FishGenome>, List<FishGenome>, List<FishGenome>> { };
    public static PopulationEvent onFishPopulationChanged = new PopulationEvent();
}
