using System.Collections.Generic;
using UnityEngine;

/**
 * Manages pace the game runs at
 */
public class TimeManager : MonoBehaviour
{
    // states representing each possible time rate the game can run at
    public enum TimeState
    {
        Paused,
        NormalSpeed,
        FasterSpeed,
        FastestSpeed
    }

    // singleton instance of the time manager
    public static TimeManager Instance { get; private set; }

    // time rate the game is currently running at
    private TimeState timeState;

    // list of pausable objects in the game
    private List<IPausable> pausableObjects;

    // slightly sped up time scale
    [Range(0f, 10f)]
    public float fasterTimeScale;

    // majorly sped up time scale
    [Range(0f, 10f)]
    public float fastestTimeScale;

    #region Major Monobehaviour Functions

    /**
     * Initialization function
     */
    private void Awake()
    {
        // handle singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one TimeManager in scene, deleting...");
            Destroy(this);
        }

        // init list of pausables
        pausableObjects = new List<IPausable>();
    }

    #endregion

    #region Pausable Objects

    /**
     * Register a pausable object with the manager
     */
    public void RegisterPausable(IPausable pausable)
    {
        if (timeState == TimeState.Paused)
        {
            pausable.Pause();
        }
        else
        {
            pausable.Resume();
        }

        pausableObjects.Add(pausable);
    }

    #endregion

    #region Time Functions

    /**
     * Pause the game
     */
    public void Pause()
    {
        timeState = TimeState.Paused;

        foreach(IPausable p in pausableObjects)
        {
            p.Pause();
        }
    }

    /**
     * Make the game run at normal time
     */
    public void NormalTime()
    {
        Resume();

        timeState = TimeState.NormalSpeed;

        Time.timeScale = 1;
    }

    /**
     * Make the game run at a slightly faster speed
     */
    public void FasterTime()
    {
        Resume();

        timeState = TimeState.FasterSpeed;

        Time.timeScale = fasterTimeScale;
    }

    /**
     * Make the game run at a very fast speed
     */
    public void FastestTime()
    {
        Resume();

        timeState = TimeState.FastestSpeed;

        Time.timeScale = fastestTimeScale;
    }

    /**
     * Unpause all paused objects if necessary
     */
    private void Resume()
    {
        if (timeState == TimeState.Paused)
        {
            foreach (IPausable p in pausableObjects)
            {
                p.Resume();
            }
        }
    }

    #endregion
}
