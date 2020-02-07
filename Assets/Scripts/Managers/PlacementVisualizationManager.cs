using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Managers visualization of placement locations for placeable objects in the game.
 */
public class PlacementVisualizationManager : MonoBehaviour
{
    // singleton instance
    public static PlacementVisualizationManager Instance { get; private set; }

    /**
     * Initialization function
     */
    private void Awake()
    {
        // manage singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one PlacementVisualizationManager in the scene! Deleting...");
            Destroy(this);
        }
    }

    /**
     * Turn a subset of visualizations on or off
     * 
     * @param type System.Type The type of the object we are displaying visualizations for
     * @param activate bool True if we want to activate the visualizations, false if we want to deactivate
     */
    public void DisplayVisualization(System.Type type, bool activate)
    {
        // look through different types to figure out which visualizations to turn on and off
        if (type == typeof(Dam))
        {
            // show placement locations for dams
            DamPlacementLocation.SetDamVisualizations(activate);
        }
        else if (type == typeof(DamLadder))
        {
            // show placement locations for dam ladders
            DamPlacementLocation.SetLadderVisualizations(activate);
        }
    }
}
