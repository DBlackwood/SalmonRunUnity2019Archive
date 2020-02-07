using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Script that controls the overall game flow.
 */
[RequireComponent(typeof(TimeManager))]
public partial class GameManager : MonoBehaviour
{
    // singleton instance of the manager
    public static GameManager Instance { get; private set; }

    // what turn of the game we are currently on
    private int m_turn;
    public int Turn
    {
        get
        {
            return m_turn;
        }

        set
        {
            m_turn = value;
            GameEvents.onTurnUpdated.Invoke();
        }
    }

    // current state of the game
    private GameState currentState;

    // fish school in the game
    public FishSchool school;

    // time manager script
    private TimeManager timeManager;

    #region Major MonoBehavior Functions

    /**
     * Initialization function
     */
    private void Awake()
    {
        // set as singleton or delete if there's already a singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one game manager in the scene! Deleting second manager...");
            Destroy(this);
        }
    }

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // get refs to other components
        timeManager = GetComponent<TimeManager>();

        // set to initial state
        Turn = 1;
        timeManager.Pause();
    }

    /**
     * Called each frame update
     */
    private void Update()
    {
        // do any update-level operations required in the current state
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    #endregion

    #region State Management

    /**
     * Change from one state to another
     */
    public void SetState(GameState newState)
    {
        // if there is a previous state, exit it properly
        if (currentState != null)
        {
            currentState.ExitState();
        }

        // hold on to old state
        GameState oldState = currentState;

        // update the state
        currentState = newState;

        // enter the new state
        currentState.Enter(oldState);

        Debug.Log(" -> " +  currentState.GetType().Name);
    }

    #endregion

    #region UI Functions

    /**
     * Start the game from the intro panel
     */
    public void StartButton()
    {
        // only does anything if we're at the very beginning
        // if so, enter the place state
        if (currentState == null)
        {
            SetState(new PlaceState());
        }
    }

    /**
     * Pause the game
     */
    public void PauseButton()
    {
        // can only pause during the run itself
        if (currentState.GetType() == typeof(RunState))
        {
            // pause the game
            timeManager.Pause();
        }
    }

    /**
     * Put the game into normal speed
     */
    public void PlayButton()
    {
        // what the play button shoud do is based on what the current state is
        switch(currentState.GetType().Name)
        {
            case nameof(PlaceState):
                SetState(new RunState());
                break;
            case nameof(RunState):
            default:
                NormalSpeed();
                break;
        }
    }

    /**
     * Put game at normal speed
     */
    public void NormalSpeed()
    {
        timeManager.NormalTime();
    }

    /**
     * Put game into faster speed
     */
    public void FasterSpeed()
    {
        // only make diff speed available during the run
        if (currentState.GetType() == typeof(RunState))
        {
            timeManager.FasterTime();
        }
    }

    /**
     * Put game into fastest speed
     */
    public void FastestSpeed()
    {
        // only make diff speed available during the run
        if (currentState.GetType() == typeof(RunState))
        {
            timeManager.FastestTime();
        }
    }

    #endregion
}
