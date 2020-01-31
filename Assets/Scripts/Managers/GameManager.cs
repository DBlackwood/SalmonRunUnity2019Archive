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

    // name of the previous game state
    private GameState prevState;

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

        // check for keyboard input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentState.GetType().Name)
            {
                case nameof(RunState):
                    // for now, this will skip to the run stats state (in case one of the fish gets stuck or something like that
                    school.KillAllActive();
                    break;
            }
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
        prevState = currentState;

        // update the state
        currentState = newState;

        // enter the new state
        currentState.Enter(prevState);

        Debug.Log(" -> " +  currentState.GetType().Name);
    }

    /**
     * Go back to the previous state
     */
    public void RevertState()
    {
        SetState(prevState);
    }

    /**
     * Get the current game state
     */
    public string GetStateName()
    {
        return currentState.GetType().Name;
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
            SetState(new RunStatsState());
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
            Pause();
        }
    }

    /**
     * React to player pressing the "Play" button
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
     * React to player pressing the "Stop" button
     */
    public void StopButton()
    {
        SetState(new EndState(EndGame.Reason.ManualQuit));
    }

    /**
     * Pause the game
     */
    public void Pause()
    {
        timeManager.Pause();
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
