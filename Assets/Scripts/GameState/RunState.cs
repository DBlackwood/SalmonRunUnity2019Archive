using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state during the actual running of the salmon
 */
public class RunState : GameState
{
    /**
     * Handle entry into the Run state
     */
    public override void Enter(GameState oldState)
    {
        // set to normal speed by default
        GameManager.Instance.NormalSpeed();

        // fire event to inform that run has started
        GameEvents.onStartRun.Invoke();
    }

    /**
     * Handle exiting the Run state
     */
    public override void ExitState()
    {
        // increment turn
        GameManager.Instance.Turn++;

        // fire event to inform that run has ended
        GameEvents.onEndRun.Invoke();
    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {
        
    }
}
