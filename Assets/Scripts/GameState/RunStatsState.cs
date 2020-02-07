using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state where statistics are displayed (presumably following the Run state)
 */
public class RunStatsState : GameState
{
    /**
     * Handle entry into the RunStats state
     */
    public override void Enter(GameState oldState)
    {
        // invoke event
        GameEvents.onStartRunStats.Invoke();
    }

    /**
     * Handle exiting the RunStats state
     */
    public override void ExitState()
    {

    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {

    }
}
