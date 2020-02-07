using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Script that controls the overall game flow.
 */
[RequireComponent(typeof(TimeManager))]
public class GameManager : MonoBehaviour
{
    // states that the game can be in
    public enum GameState
    {
        Intro,
        Place,
        Run
    }

    // singleton instance of the manager
    public static GameManager Instance { get; private set; }

    // event for starting a run
    public static UnityEvent onStartRun = new UnityEvent();

    // event for ending a run
    public static UnityEvent onEndRun = new UnityEvent();

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
            uiUpdater.UpdateUI();
        }
    }

    // fish school in the game
    public FishSchool school;

    // state the game is currently in
    private GameState gameState;

    // time manager script
    private TimeManager timeManager;

    // UI updater
    private UIUpdater uiUpdater = new UIUpdater();

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

        // initialize ui updater
        uiUpdater.Init();
    }

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // get refs to other components
        timeManager = GetComponent<TimeManager>();

        // set to initial state
        gameState = GameState.Intro;
        Turn = 1;
        uiUpdater.UpdateUI();
        timeManager.Pause();
    }

    #endregion

    #region UI Functions

    /**
     * Start the game from the intro panel
     */
    public void StartButton()
    {
        switch(gameState)
        {
            case GameState.Intro:
                ToPlace();
                break;
        }
    }

    /**
     * Pause the game
     */
    public void PauseButton()
    {
        // pause the game
        timeManager.Pause();
    }

    /**
     * Put the game into normal speed
     */
    public void PlayButton()
    {
        switch(gameState)
        {
            case GameState.Intro:
                break;
            case GameState.Place:
                ToRun();
                break;
            case GameState.Run:
            default:
                timeManager.NormalTime();
                break;
        }
    }

    /**
     * Put game into faster speed
     */
    public void FasterButton()
    {
        if (gameState != GameState.Place)
        {
            timeManager.FasterTime();
        }
    }

    /**
     * Put game into fastest speed
     */
    public void FastestButton()
    {
        if (gameState != GameState.Place)
        {
            timeManager.FastestTime();
        }
    }

    #endregion

    /**
     * Move to the Place state
     */
    public void ToPlace()
    {
        // hold the old state
        GameState prevGameState = gameState;

        // switch to the new state
        gameState = GameState.Place;

        // if we were previously in the run state, add a turn
        if (prevGameState == GameState.Run)
        {
            onEndRun.Invoke();
            gameState = GameState.Place;
            Turn++;
        }
    }

    /**
     * Move to the Run state
     */
    private void ToRun()
    {
        // hold the previous state
        GameState prevGameState = gameState;

        // update the state
        gameState = GameState.Run;

        onStartRun.Invoke();
        PlayButton();
    }

}
