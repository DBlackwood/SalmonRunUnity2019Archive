using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state during the phase designated for ending of the game
 */
public class EndState : GameState
{
    // reason why we are quitting
    private EndGame.Reason reason;

    /**
     * Constructor
     * 
     * @param reason The reason for ending the game
     */
    public EndState(EndGame.Reason reason)
    {
        this.reason = reason;
    }

    /**
     * Handle entry into the Place state
     */
    public override void Enter(GameState oldState)
    {
        // pause the game
        GameManager.Instance.PauseButton();

        // fire game end event
        GameEvents.onEndGame.Invoke(reason);
    }

    /**
     * Handle exiting the Place state
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
