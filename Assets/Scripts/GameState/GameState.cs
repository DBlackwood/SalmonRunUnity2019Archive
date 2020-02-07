using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Abstract base class for a state that the game can be in
 */
public abstract class GameState
{
    /**
     * Action take on state entry
     */
    public abstract void Enter(GameState oldState);

    /**
     * Action taken repeatedly during a state
     */
    public abstract void UpdateState();

    /**
     * Action taken on state exit
     */
    public abstract void ExitState();
}
